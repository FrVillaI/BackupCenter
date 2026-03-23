using System.Threading.Tasks;
namespace BackupCenter.Application.Interfaces;

public interface IDbfImportService
{
    Task ImportAsync(string dbfPath);
}
