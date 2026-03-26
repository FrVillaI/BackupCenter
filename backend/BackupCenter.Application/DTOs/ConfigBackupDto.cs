namespace BackupCenter.Application.DTOs;

public class ConfigBackupDto
{
    public int FrecuenciaHoras { get; set; } // 12 o 24
    public TimeSpan HoraProgramada { get; set; }
}