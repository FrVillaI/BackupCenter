using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BackupCenter.Data;
using BackupCenter.Domain.Entities;
using BackupCenter.Application.Interfaces;
using NDbfReader;

namespace BackupCenter.Application.Services;

public class DbfImportService : IDbfImportService
{
    private readonly BackupCenterDbContext _db;
    private readonly ILogger<DbfImportService> _logger;

    public DbfImportService(BackupCenterDbContext db, ILogger<DbfImportService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ImportAsync(string dbfPath)
    {
        if (string.IsNullOrWhiteSpace(dbfPath))
        {
            _logger.LogWarning("Ruta DBF vacía o nula.");
            return;
        }

        if (!File.Exists(dbfPath))
        {
            _logger.LogError("Archivo DBF no encontrado: {Path}", dbfPath);
            return;
        }

        try
        {
            using var table = Table.Open(dbfPath);
            var reader = table.OpenReader();

            while (reader.Read())
            {
                var nombre = reader.GetString("EMPRESA")?.Trim()
                             ?? reader.GetString("NOMBRE")?.Trim();

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    _logger.LogWarning("Registro ignorado por nombre vacío.");
                    continue;
                }

                var ruta = reader.GetString("RUTA_ORIGEN") ?? reader.GetString("RUTA");
                var frecuencia = reader.GetString("FRECUENCIA");
                var hora = reader.GetString("HORA") ?? reader.GetString("HORARIO");

                int activo = SafeGetInt(reader, "ACTIVO");
                DateTime? ultima = SafeGetDate(reader, "ULTIMA_COPIA");

                var existing = await _db.Empresas
                    .FirstOrDefaultAsync(e => e.Nombre == nombre);

                if (existing != null)
                {
                    existing.RutaOrigen = ruta ?? existing.RutaOrigen;
                    existing.Activa = activo != 0;
                    existing.Frecuencia = frecuencia ?? existing.Frecuencia;
                    existing.HoraProgramada = hora ?? existing.HoraProgramada;
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
                        Frecuencia = frecuencia ?? string.Empty,
                        HoraProgramada = hora ?? string.Empty,
                        UltimaCopia = ultima
                    });
                }
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Importación DBF completada: {Path}", dbfPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al importar DBF: {Path}", dbfPath);
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