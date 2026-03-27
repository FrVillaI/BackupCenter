namespace BackupCenter.Domain.Entities;

/// Representa una empresa dentro del sistema.
/// Define la configuración de backups automáticos y manuales.
public class Empresa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    /// Ruta del sistema de archivos que será respaldada.
    /// Debe ser una ruta válida en el servidor.
    public string RutaOrigen { get; set; } = string.Empty;

    /// Indica si la empresa está activa.
    /// Solo empresas activas pueden generar backups.
    public bool Activa { get; set; } = true;

    /// Frecuencia en horas para la ejecución de backups automáticos.
    /// Ejemplos: 12 (cada 12 horas), 24 (una vez al día).
    public int FrecuenciaHoras { get; set; }

    /// Hora específica del día en la que se ejecuta el backup automático.
    public TimeSpan HoraProgramada { get; set; }

    /// Fecha y hora de la última copia realizada.
    /// Se utiliza para calcular la próxima ejecución automática.
    public DateTime? UltimaCopia { get; set; }
}
