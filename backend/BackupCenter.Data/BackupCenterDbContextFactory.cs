using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BackupCenter.Data;

public class BackupCenterDbContextFactory : IDesignTimeDbContextFactory<BackupCenterDbContext>
{
    public BackupCenterDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BackupCenterDbContext>();
        optionsBuilder.UseSqlite("Data Source=BackupCenter.db");

        return new BackupCenterDbContext(optionsBuilder.Options);
    }
}