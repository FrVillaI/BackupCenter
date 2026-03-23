using System;

namespace BackupCenter.Domain.Entities;

public class LogEntry
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Resultado { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;
    public string IpEquipo { get; set; } = string.Empty;
}
