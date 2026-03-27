using System;

namespace BackupCenter.Domain.Entities;

/// Representa un registro de auditoría del sistema.
/// Guarda información sobre acciones realizadas por usuarios o procesos automáticos.
public class LogEntry
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;

    /// Tipo de acción realizada.
    /// Ejemplos: BACKUP_MANUAL, BACKUP_AUTOMATICO, LOGIN, ERROR.
    public string Accion { get; set; } = string.Empty;

    /// Resultado de la acción.
    /// Valores típicos: OK, ERROR.
    public string Resultado { get; set; } = string.Empty;

    /// Información adicional o detalle del evento.
    public string Detalle { get; set; } = string.Empty;

    /// Dirección IP del equipo desde donde se ejecutó la acción.
    /// Puede estar vacío en procesos automáticos.
    public string IpEquipo { get; set; } = string.Empty;
}
