namespace BackupCenter.Application.Models;

/// Resultado de la ejecución de un backup.
/// Contiene la información del archivo generado y su hash.
public class BackupResult
{
    /// Ruta completa del archivo ZIP generado.
    public string ZipPath { get; set; } = string.Empty;

    /// Ruta completa del archivo ZIP generado.
    public string Hash { get; set; } = string.Empty;

    /// Hash SHA-256 del archivo ZIP.
    public string HashPath { get; set; } = string.Empty;
}