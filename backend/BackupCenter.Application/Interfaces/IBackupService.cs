using System.Threading.Tasks;
using BackupCenter.Application.Models;

namespace BackupCenter.Application.Interfaces;

public interface IBackupService
{
    Task<BackupResult> CreateBackupAsync(int empresaId, string? overridePath = null, bool isAutomatic = false);
}
