using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using BackupCenter.Application.Interfaces;
using BackupCenter.Application.Models;
using BackupCenter.Domain.Entities;
using BackupCenter.Data;

namespace BackupCenter.Application.Services;

public class BackupService : IBackupService
{
    private readonly BackupCenterDbContext _db;
    private readonly string _backupRoot = @"C:\Fenix\Backups";
    private const long MaxCopyBytes = 50L * 1024 * 1024 * 1024; // 50 GB
    public BackupService(BackupCenterDbContext db)
    {
        _db = db;
        Directory.CreateDirectory(_backupRoot);
    }

    public async Task<BackupResult> CreateBackupAsync(int empresaId, string? overridePath = null, bool isAutomatic = false)
    {
        var empresa = await _db.Empresas.FirstOrDefaultAsync(e => e.Id == empresaId);
        if (empresa == null) throw new Exception("Empresa no encontrada");

        var sourcePath = string.IsNullOrWhiteSpace(overridePath) ? empresa.RutaOrigen : overridePath;
        if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
            throw new DirectoryNotFoundException("Ruta origen no válida o no encontrada");

        // Tamaño de origen chequeado
        var totalBytes = GetDirectorySize(sourcePath);
        if (totalBytes > MaxCopyBytes)
            throw new InvalidOperationException("El tamaño de la carpeta origen excede el límite permitido para respaldo.");

        var empresaFolderName = empresa.Nombre.Replace(" ", "_");
        var empresaBasePath = Path.Combine(_backupRoot, empresaFolderName);
        Directory.CreateDirectory(empresaBasePath);

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var zipFileName = $"Backup_{empresaFolderName}_{timestamp}.zip";
        var stagingDir = Path.Combine(empresaBasePath, "staging_" + timestamp);
        Directory.CreateDirectory(stagingDir);

        // Copiar carpeta origen a staging
        try
        {
            CopyDirectory(sourcePath, stagingDir);
        }
        catch (UnauthorizedAccessException uaex)
        {
            throw new UnauthorizedAccessException("Permisos insuficientes para leer/copiar la ruta origen.", uaex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al copiar datos para backup: {ex.Message}", ex);
        }

        // Crear ZIP
        var zipPath = Path.Combine(empresaBasePath, zipFileName);
        try
        {
            ZipFile.CreateFromDirectory(stagingDir, zipPath, CompressionLevel.Optimal, true);
        }
        catch (Exception ex)
        {
            // Limpiar staging si falla
            try { Directory.Delete(stagingDir, true); } catch { }
            throw new IOException("Error al crear ZIP del backup: " + ex.Message, ex);
        }

        // Quitar staging
        try { Directory.Delete(stagingDir, true); } catch { /* ignore */ }

        // Hash SHA-256 del ZIP
        string hash;
        try
        {
            hash = ComputeSha256(zipPath);
            var hashPath = zipPath + ".sha256";
            await File.WriteAllTextAsync(hashPath, hash);
        }
        catch (Exception ex)
        {
            throw new IOException("Error al calcular/guardar hash del ZIP: " + ex.Message, ex);
        }

        // Registrar en DB
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
        await _db.SaveChangesAsync();

        // Registro de log básico
        var log = new LogEntry {
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

        return new BackupResult { ZipPath = zipPath, Hash = hash, HashPath = zipPath + ".sha256" };
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
                try { size += new FileInfo(f).Length; } catch { /* ignore inaccessible files */ }
            }
        }
        catch { }
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
            var dest = filePath.Replace(sourceDir, destDir);
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.Copy(filePath, dest, true);
        }
    }
}
