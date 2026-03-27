using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BackupCenter.Domain.Entities;
using BackupCenter.Data;
using Microsoft.EntityFrameworkCore;
using BackupCenter.Application.Interfaces;
using BackupCenter.Application.Services;

namespace BackupCenter.Application.Services;

/// Servicio encargado de la autenticación de usuarios y generación de tokens JWT
public class AuthService : IAuthService
{
    private readonly BackupCenterDbContext _db;
    private readonly JwtSettings _jwtSettings;

    /// Constructor del servicio de autenticación.
    public AuthService(BackupCenterDbContext db, JwtSettings jwtSettings)
    {
        _db = db;
        _jwtSettings = jwtSettings;
    }

    /// Crea usuarios por defecto si la base de datos está vacía.
    /// Se ejecuta normalmente al iniciar la aplicación.
    public async Task SeedAdminIfNeeded()
    {
        if (!await _db.Usuarios.AnyAsync())
        {
            var user1 = new User { Username = "ads", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = "ADMIN", Activo = true, FechaCreacion = DateTime.UtcNow };
            var user2 = new User { Username = "gerente", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = "GERENTE", Activo = true, FechaCreacion = DateTime.UtcNow };
            _db.Usuarios.Add(user1);
            _db.Usuarios.Add(user2);
            await _db.SaveChangesAsync();
        }
    }

    /// Obtiene un usuario por su nombre de usuario.
    public async Task<User?> GetUserAsync(string username)
    {
        return await _db.Usuarios.FirstOrDefaultAsync(u => u.Username == username);
    }

    /// Valida las credenciales del usuario y genera un token JWT si son correctas.
    /// Token JWT si es válido; null en caso contrario
    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        var user = await GetUserAsync(username);

        // Validación de usuario y contraseña
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;
        // Crear claims (datos dentro del token)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };
        // Generar clave de firma
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // Crear token
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiryHours),
            signingCredentials: creds
        );
        // Serializar token a string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
