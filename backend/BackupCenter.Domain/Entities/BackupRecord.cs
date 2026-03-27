namespace BackupCenter.Domain.Entities;

/// Representa un registro de backup generado por el sistema.
/// Contiene información del archivo, integridad y tipo de ejecución.
public class BackupRecord
{
    public int Id { get; set; }
    public string Empresa { get; set; } = string.Empty;

    /// Fecha y hora en que se generó el backup
    public DateTime Fecha { get; set; }

    /// Nombre del archivo generado (ZIP).
    public string Archivo { get; set; } = string.Empty;

    /// Hash SHA-256 del archivo de backup.
    /// Se utiliza para verificar la integridad del archivo.
    public string Hash { get; set; } = string.Empty;

    /// Ruta completa donde se almacena el archivo de backup.
    public string Ruta { get; set; } = string.Empty;

    /// Tipo de backup realizado.
    /// Valores típicos: MANUAL, AUTOMATICO.
    public string Tipo { get; set; } = string.Empty;
}
