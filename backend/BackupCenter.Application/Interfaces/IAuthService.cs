using BackupCenter.Domain.Entities;
using System.Threading.Tasks;

namespace BackupCenter.Application.Interfaces;

public interface IAuthService
{
    Task<string?> AuthenticateAsync(string username, string password);
    Task SeedAdminIfNeeded();
    Task<User?> GetUserAsync(string username);
}
