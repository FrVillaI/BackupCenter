using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BackupCenter.Data;

/// Factory para crear instancias de BackupCenterDbContext en tiempo de diseño.
/// Utilizada por Entity Framework Core para ejecutar migraciones y comandos CLI.
public class BackupCenterDbContextFactory : IDesignTimeDbContextFactory<BackupCenterDbContext>
{
    /// Crea una instancia de BackupCenterDbContext con configuración básica.
    /// Este método es utilizado exclusivamente por herramientas como 'dotnet ef'.
    public BackupCenterDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BackupCenterDbContext>();

        // Configuración de base de datos SQLite para migraciones
        optionsBuilder.UseSqlite("Data Source=BackupCenter.db");

        return new BackupCenterDbContext(optionsBuilder.Options);
    }
}