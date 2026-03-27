namespace BackupCenter.Domain.Entities;

/// Representa un usuario del sistema.
/// Controla el acceso a funcionalidades según su rol.
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;

    /// Hash de la contraseña (no se almacena la contraseña en texto plano).
    public string PasswordHash { get; set; } = string.Empty;

    /// Rol del usuario dentro del sistema.
    public string Role { get; set; } = "USER"; // ADMIN o GERENTE
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
