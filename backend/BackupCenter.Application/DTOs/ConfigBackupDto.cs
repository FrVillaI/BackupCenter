namespace BackupCenter.Application.DTOs;

/// DTO utilizado para configurar la frecuencia y hora de ejecución de backups.
public class ConfigBackupDto
{
    public int FrecuenciaHoras { get; set; } // 12 o 24
    public TimeSpan HoraProgramada { get; set; }
}