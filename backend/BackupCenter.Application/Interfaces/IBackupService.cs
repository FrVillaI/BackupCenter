using System.Threading.Tasks;
using BackupCenter.Application.Models;

namespace BackupCenter.Application.Interfaces;

/// Define las operaciones relacionadas con la generación de backups.
public interface IBackupService
{
    /// Crea un backup de la empresa especificada.
    Task<BackupResult> CreateBackupAsync(int empresaId, string? overridePath = null, bool isAutomatic = false);
}
