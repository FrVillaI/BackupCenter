using Microsoft.EntityFrameworkCore;
using BackupCenter.Domain.Entities;

namespace BackupCenter.Data;

/// Contexto principal de base de datos.
/// Gestiona el acceso a las entidades y la configuración del modelo.
public class BackupCenterDbContext : DbContext
{
    /// Constructor que recibe las opciones de configuración del contexto.
    public BackupCenterDbContext(DbContextOptions<BackupCenterDbContext> options) : base(options) { }

    /// Tabla de usuarios del sistema.
    public DbSet<User> Usuarios { get; set; } = default!;

    /// Tabla de empresas configuradas para backup.
    public DbSet<Empresa> Empresas { get; set; } = default!;

    /// Tabla de empresas configuradas para backup.
    public DbSet<LogEntry> Logs { get; set; } = default!;

    /// Tabla de logs del sistema (auditoría).
    public DbSet<BackupRecord> Backups { get; set; } = default!;

    /// Configuración adicional del modelo y datos iniciales (seed).
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Seed de usuarios iniciales
        // Nota: las contraseñas están hasheadas con BCrypt
        var user1 = new User { Id = 1, Username = "ads", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = "ADMIN", Activo = true, FechaCreacion = DateTime.UtcNow };
        var user2 = new User { Id = 2, Username = "gerente", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = "GERENTE", Activo = true, FechaCreacion = DateTime.UtcNow };
        modelBuilder.Entity<User>().HasData(user1, user2);
    }
}
