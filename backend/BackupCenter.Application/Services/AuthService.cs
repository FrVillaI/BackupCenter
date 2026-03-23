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

public class AuthService : IAuthService
{
    private readonly BackupCenterDbContext _db;
    private readonly JwtSettings _jwtSettings;
    public AuthService(BackupCenterDbContext db, JwtSettings jwtSettings)
    {
        _db = db;
        _jwtSettings = jwtSettings;
    }

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

    public async Task<User?> GetUserAsync(string username)
    {
        return await _db.Usuarios.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        var user = await GetUserAsync(username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;
        // Crear claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiryHours),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
