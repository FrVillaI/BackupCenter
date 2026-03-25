using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BackupCenter.Data;
using BackupCenter.Domain.Entities;
using BackupCenter.Application.Interfaces;
using BackupCenter.Application.DTOs;
using NDbfReader;

namespace BackupCenter.Application.Services
{
    public class DbfImportService : IDbfImportService
    {
        private readonly BackupCenterDbContext _db;
        private readonly ILogger<DbfImportService> _logger;

        public DbfImportService(BackupCenterDbContext db, ILogger<DbfImportService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ImportResult> ImportAsync(string dbfPath)
        {
            var result = new ImportResult();

            if (string.IsNullOrWhiteSpace(dbfPath))
            {
                _logger.LogWarning("Ruta DBF vacía o nula.");
                return result;
            }

            if (!File.Exists(dbfPath))
            {
                _logger.LogError("Archivo DBF no encontrado: {Path}", dbfPath);
                return result;
            }

            try
            {
                using var table = Table.Open(dbfPath);
                var reader = table.OpenReader();

                // Log de columnas para debug
                foreach (var col in table.Columns)
                    _logger.LogInformation("Campo DBF detectado: {Field} ({Type})", col.Name, col.Type);

                while (reader.Read())
                {
                    result.Leidos++;

                    try
                    {
                        // Campos correctos según tu EMPRESAS.DBF
                        var nombre = reader.GetString("EMPRESA")?.Trim();
                        var ruta = reader.GetString("RUTA")?.Trim();
                        int activo = SafeGetInt(reader, "ACTIVOS");
                        DateTime? ultima = SafeGetDate(reader, "FTP_ULTIMO"); // ejemplo de fecha

                        if (string.IsNullOrWhiteSpace(nombre))
                        {
                            result.Errores++;
                            continue;
                        }

                        // Buscar existente por nombre
                        var existing = await _db.Empresas
                            .FirstOrDefaultAsync(e => e.Nombre == nombre);

                        if (existing != null)
                        {
                            existing.RutaOrigen = ruta ?? existing.RutaOrigen;
                            existing.Activa = activo != 0;
                            existing.UltimaCopia = ultima;

                            _db.Empresas.Update(existing);
                        }
                        else
                        {
                            _db.Empresas.Add(new Empresa
                            {
                                Nombre = nombre,
                                RutaOrigen = ruta ?? string.Empty,
                                Activa = activo != 0,
                                UltimaCopia = ultima
                            });

                            result.Insertados++;
                        }
                    }
                    catch (Exception exRow)
                    {
                        result.Errores++;
                        _logger.LogWarning(exRow, "Error procesando registro DBF.");
                    }
                }

                await _db.SaveChangesAsync();

                _logger.LogInformation(
                    "Importación completada. Leídos: {Leidos}, Insertados: {Insertados}, Errores: {Errores}",
                    result.Leidos, result.Insertados, result.Errores
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al importar DBF: {Path}", dbfPath);
                throw;
            }
        }

        // Métodos auxiliares seguros
        private int SafeGetInt(Reader reader, string field)
        {
            try { return reader.GetInt32(field); }
            catch { return 0; }
        }

        private DateTime? SafeGetDate(Reader reader, string field)
        {
            try { return reader.GetDateTime(field); }
            catch { return null; }
        }
    }
}