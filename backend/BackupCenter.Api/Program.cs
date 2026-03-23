using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using BackupCenter.Data;
using BackupCenter.Application.Services;
using BackupCenter.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BackupCenter.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// JWT
var jwtSecret = configuration["JwtConfig:SecretKey"] ?? "CHANGE_ME";
var issuer = configuration["JwtConfig:Issuer"] ?? "BackupCenter";
var audience = configuration["JwtConfig:Audience"] ?? "BackupCenterClient";

// DB
builder.Services.AddDbContext<BackupCenterDbContext>(options =>
    options.UseSqlite("Data Source=BackupCenter.db"));

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = key
    };
});

builder.Services.AddAuthorization();

// Servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBackupService, BackupService>();
builder.Services.AddScoped<IDbfImportService, DbfImportService>();

builder.Services.AddHostedService<BackupCenter.Infrastructure.Background.BackupSchedulerService>();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();