using System.Threading.Tasks;
using BackupCenter.Application.DTOs;

namespace BackupCenter.Application.Interfaces;

public interface IDbfImportService
{
    Task<ImportResult> ImportAsync(string path);
}