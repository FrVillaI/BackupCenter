using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using BackupCenter.Application.Interfaces;
using BackupCenter.Application.Models;
using BackupCenter.Domain.Entities;
using BackupCenter.Data;

namespace BackupCenter.Application.Services;

public class BackupService : IBackupService
{
    private readonly BackupCenterDbContext _db;
    private readonly ILogger<BackupService> _logger;
    private readonly string _backupRoot;
    private const long MaxCopyBytes = 50L * 1024 * 1024 * 1024; // 50 GB

    // Lock por empresa
    private static readonly ConcurrentDictionary<int, SemaphoreSlim> _empresaLocks = new();

    public BackupService(BackupCenterDbContext db, ILogger<BackupService> logger, IConfiguration config)
    {
        _db = db;
        _logger = logger;

        _backupRoot = config["BackupSettings:RootPath"]
              ?? throw new Exception("Backup root path no configurado");

        Directory.CreateDirectory(_backupRoot);
    }

    public async Task<BackupResult> CreateBackupAsync(int empresaId, string? overridePath = null, bool isAutomatic = false)
    {
        var empresa = await _db.Empresas.FirstOrDefaultAsync(e => e.Id == empresaId);
        if (empresa == null)
            throw new Exception($"Empresa {empresaId} no encontrada");

        if (!empresa.Activa)
            throw new InvalidOperationException($"La empresa '{empresa.Nombre}' está inactiva y no puede generar backups");

        // Lock por empresa para evitar concurrencia
        var semaphore = _empresaLocks.GetOrAdd(empresaId, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();

        try
        {
            var sourcePath = string.IsNullOrWhiteSpace(overridePath) ? empresa.RutaOrigen : overridePath;

            if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
                throw new DirectoryNotFoundException("Ruta origen no válida o no encontrada");

            var totalBytes = GetDirectorySize(sourcePath);
            if (totalBytes > MaxCopyBytes)
                throw new InvalidOperationException("El tamaño de la carpeta origen excede el límite permitido");

            var empresaFolderName = empresa.Nombre.Replace(" ", "_");
            var empresaBasePath = Path.Combine(_backupRoot, empresaFolderName);
            Directory.CreateDirectory(empresaBasePath);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var zipFileName = $"Backup_{empresaFolderName}_{timestamp}.zip";
            var stagingDir = Path.Combine(empresaBasePath, "staging_" + timestamp);
            Directory.CreateDirectory(stagingDir);

            // 🔄 Copiar carpeta origen
            try
            {
                CopyDirectory(sourcePath, stagingDir);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error copiando archivos para backup de {Empresa}", empresa.Nombre);
                throw;
            }

            // 🔄 Crear ZIP
            var zipPath = Path.Combine(empresaBasePath, zipFileName);
            try
            {
                ZipFile.CreateFromDirectory(stagingDir, zipPath, CompressionLevel.Optimal, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando ZIP para backup de {Empresa}", empresa.Nombre);
                throw;
            }
            finally
            {
                // 🔄 Limpiar staging
                try { Directory.Delete(stagingDir, true); } catch { }
            }

            // 🔄 Hash SHA-256
            string hash = ComputeSha256(zipPath);
            var hashPath = zipPath + ".sha256";
            await File.WriteAllTextAsync(hashPath, hash);

            // 🔄 Registrar en DB
            var backup = new BackupRecord
            {
                Empresa = empresa.Nombre,
                Fecha = DateTime.UtcNow,
                Archivo = Path.GetFileName(zipPath),
                Hash = hash,
                Ruta = zipPath,
                Tipo = isAutomatic ? "AUTOMATICO" : "MANUAL"
            };
            _db.Backups.Add(backup);

            // 🔥 Actualizar UltimaCopia solo una vez
            empresa.UltimaCopia = DateTime.UtcNow;

            var log = new LogEntry
            {
                Fecha = DateTime.UtcNow,
                Usuario = isAutomatic ? "SYSTEM" : "AUTOMATION",
                Empresa = empresa.Nombre,
                Accion = isAutomatic ? "BACKUP_AUTOMATICO" : "BACKUP_MANUAL",
                Resultado = "OK",
                Detalle = $"Archivo={Path.GetFileName(zipPath)}",
                IpEquipo = ""
            };
            _db.Logs.Add(log);

            await _db.SaveChangesAsync();

            _logger.LogInformation("Backup completado para {Empresa}. Archivo: {Archivo}", empresa.Nombre, zipPath);

            return new BackupResult { ZipPath = zipPath, Hash = hash, HashPath = hashPath };
        }
        finally
        {
            semaphore.Release();
        }
    }

    private string ComputeSha256(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha256.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }

    private long GetDirectorySize(string path)
    {
        long size = 0;
        try
        {
            foreach (var f in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                try { size += new FileInfo(f).Length; }
                catch (Exception ex) { _logger.LogWarning(ex, "Archivo inaccesible: {File}", f); }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculando tamaño de directorio {Path}", path);
        }
        return size;
    }

    private void CopyDirectory(string sourceDir, string destDir)
    {
        foreach (var dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            var sub = dirPath.Replace(sourceDir, destDir);
            Directory.CreateDirectory(sub);
        }
        foreach (var filePath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            try
            {
                var dest = filePath.Replace(sourceDir, destDir);
                Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                File.Copy(filePath, dest, true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error copiando archivo {File}", filePath);
            }
        }
    }
}