namespace BackupCenter.Domain.Entities;

public class BackupRecord
{
    public int Id { get; set; }
    public string Empresa { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Archivo { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public string Ruta { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
}
