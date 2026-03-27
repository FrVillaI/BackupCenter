using BackupCenter.Domain.Entities;
using System.Threading.Tasks;

namespace BackupCenter.Application.Interfaces;

/// Define las operaciones de autenticación del sistema.
public interface IAuthService
{
    /// Autentica un usuario y genera un token JWT si las credenciales son válidas.
    Task<string?> AuthenticateAsync(string username, string password);

    /// Crea usuarios por defecto si no existen en la base de datos.
    Task SeedAdminIfNeeded();

    /// Crea usuarios por defecto si no existen en la base de datos.
    Task<User?> GetUserAsync(string username);
}
