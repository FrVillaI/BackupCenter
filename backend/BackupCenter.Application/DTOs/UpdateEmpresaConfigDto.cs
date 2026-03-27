namespace BackupCenter.Application.DTOs;

/// DTO utilizado para actualizar la configuración de backups de una empresa.
public class UpdateEmpresaConfigDto
{
    public int FrecuenciaHoras { get; set; }
    public string HoraProgramada { get; set; } = "";
}