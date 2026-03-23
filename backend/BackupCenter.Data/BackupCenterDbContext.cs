using Microsoft.EntityFrameworkCore;
using BackupCenter.Domain.Entities;

namespace BackupCenter.Data;

public class BackupCenterDbContext : DbContext
{
    public BackupCenterDbContext(DbContextOptions<BackupCenterDbContext> options) : base(options) { }

    public DbSet<User> Usuarios { get; set; } = default!;
    public DbSet<Empresa> Empresas { get; set; } = default!;
    public DbSet<LogEntry> Logs { get; set; } = default!;
    public DbSet<BackupRecord> Backups { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Seed two usuarios por defecto (hash de bcrypt para palabras 'password')
        var user1 = new User { Id = 1, Username = "ads", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = "ADMIN", Activo = true, FechaCreacion = DateTime.UtcNow };
        var user2 = new User { Id = 2, Username = "gerente", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = "GERENTE", Activo = true, FechaCreacion = DateTime.UtcNow };
        modelBuilder.Entity<User>().HasData(user1, user2);
    }
}
