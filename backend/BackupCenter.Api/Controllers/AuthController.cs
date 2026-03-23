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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var token = await _auth.AuthenticateAsync(req.Username, req.Password);
        if (token == null)
            return Unauthorized();
        return Ok(new { token });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
