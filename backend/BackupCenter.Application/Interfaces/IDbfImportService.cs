using System.Threading.Tasks;
using BackupCenter.Application.DTOs;

namespace BackupCenter.Application.Interfaces;

/// Define las operaciones para importar datos desde archivos DBF.
public interface IDbfImportService
{
    /// Define las operaciones para importar datos desde archivos DBF.
    Task<ImportResult> ImportAsync(string path);
}