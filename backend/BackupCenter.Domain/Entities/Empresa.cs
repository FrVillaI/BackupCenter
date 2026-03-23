namespace BackupCenter.Domain.Entities;

public class Empresa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string RutaOrigen { get; set; } = string.Empty;
    public bool Activa { get; set; } = true;
    public string Frecuencia { get; set; } = string.Empty;
    public string HoraProgramada { get; set; } = string.Empty;
    public DateTime? UltimaCopia { get; set; }
}
