using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NDbfReader;
using BackupCenter.Data;
using BackupCenter.Domain.Entities;
using BackupCenter.Application.Interfaces;

namespace BackupCenter.Application.Services;

public class DbfImportService : IDbfImportService
{
    private readonly BackupCenterDbContext _db;

    public DbfImportService(BackupCenterDbContext db)
    {
        _db = db;
    }

    public async Task ImportAsync(string dbfPath)
    {
        if (string.IsNullOrWhiteSpace(dbfPath) || !File.Exists(dbfPath)) return;

        try
        {
            using var table = Table.Open(dbfPath);

            var reader = table.OpenReader();

            while (reader.Read())
            {
                var nombre = reader.GetString("EMPRESA")?.Trim()
                              ?? reader.GetString("NOMBRE")?.Trim();

                if (string.IsNullOrWhiteSpace(nombre)) continue;

                var ruta = reader.GetString("RUTA_ORIGEN") ?? reader.GetString("RUTA");
                var frecuencia = reader.GetString("FRECUENCIA");
                var hora = reader.GetString("HORA") ?? reader.GetString("HORARIO");

                int activo = 0;
                try { activo = reader.GetInt32("ACTIVO"); } catch { }

                DateTime? ultima = null;
                try { ultima = reader.GetDateTime("ULTIMA_COPIA"); } catch { }

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
        }
        catch (Exception ex)
        {
            // Aquí deberías loggear
            Console.WriteLine(ex.Message);
        }
    }
}