using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BackupCenter.Data;
using BackupCenter.Domain.Entities;
using BackupCenter.Application.Interfaces;

public class BackupWorker : BackgroundService
{
    private readonly IServiceProvider _sp;

    public BackupWorker(IServiceProvider sp)
    {
        _sp = sp;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _sp.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<BackupCenterDbContext>();
            var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();

            var empresas = await db.Empresas.ToListAsync();

            foreach (var e in empresas)
            {
                if (DebeEjecutar(e))
                {
                    try
                    {
                        await backupService.CreateBackupAsync(e.Id, isAutomatic: true);
                    }
                    catch
                    {
                        // luego puedes loguear
                    }
                }
            }

            // 🔥 IMPORTANTE: cada 60 segundos
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    private bool DebeEjecutar(Empresa e)
    {
        if (!e.Activa) return false;

        var ahora = DateTime.Now;
        var hoyHoraBase = DateTime.Today.Add(e.HoraProgramada);

        DateTime proxima;

        if (e.UltimaCopia == null)
        {
            proxima = hoyHoraBase;
        }
        else
        {
            proxima = e.UltimaCopia.Value.AddHours(e.FrecuenciaHoras);
        }

        return ahora >= proxima;
    }
}