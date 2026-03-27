using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BackupCenter.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BackupCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BackupCenter.Data;
using BackupCenter.Application.Services;

/// Controlador encargado de la autenticación de usuarios.
namespace BackupCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly JwtSettings _jwtSettings;
    public AuthController(IAuthService auth, JwtSettings jwtSettings)
    {
        _auth = auth;
        _jwtSettings = jwtSettings;
    }

    /// Autentica un usuario y devuelve un token JWT.
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var token = await _auth.AuthenticateAsync(req.Username, req.Password);
        if (token == null)
            return Unauthorized();
        return Ok(new { token });
    }
}

/// Representa las credenciales de inicio de sesión.
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
