namespace BackupCenter.Domain.Entities;

public class Empresa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string RutaOrigen { get; set; } = string.Empty;
    public bool Activa { get; set; } = true;
    public int FrecuenciaHoras { get; set; } // 12 o 24
    public TimeSpan HoraProgramada { get; set; }
    public DateTime? UltimaCopia { get; set; }
}
