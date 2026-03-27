namespace BackupCenter.Application.DTOs;

/// Resultado del proceso de importación de datos.
public class ImportResult
{
    /// Número total de registros leídos desde la fuente (ej: archivo DBF).
    public int Leidos { get; set; }

    /// Número de registros insertados correctamente en la base de datos.
    public int Insertados { get; set; }

    /// Resultado del proceso de importación de datos.
    public int Errores { get; set; }
}