using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BackupCenter.Application.Interfaces;
using BackupCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BackupCenter.Data;

namespace BackupCenter.Infrastructure.Background;

// Background scheduler that triggers backups at configured times (hour:minute) every day.
public class BackupSchedulerService : BackgroundService
{
    private readonly IServiceProvider _services;
    private static readonly ConcurrentDictionary<(int, DateTime), bool> _backedUpToday = new();
    public BackupSchedulerService(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                var hhmm = now.ToString("HH:mm");
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BackupCenterDbContext>();
                var backups = await db.Empresas.AsNoTracking().Where(e => e.Activa).ToListAsync(stoppingToken);
                foreach (var e in backups)
                {
                    var ahora = DateTime.Now;
                    var horaActual = ahora.TimeOfDay;

                    var diferencia = (horaActual - e.HoraProgramada).TotalMinutes;

                    if (Math.Abs(diferencia) < 1)
                    {
                        var key = (e.Id, ahora.Date);

                        if (_backedUpToday.TryAdd(key, true))
                        {
                            try
                            {
                                var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();
                                await backupService.CreateBackupAsync(e.Id, null, isAutomatic: true);
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    var log = new BackupCenter.Domain.Entities.LogEntry
                                    {
                                        Fecha = DateTime.UtcNow,
                                        Usuario = "SYSTEM",
                                        Empresa = e.Nombre,
                                        Accion = "BACKUP_AUTOMATICO",
                                        Resultado = "ERROR",
                                        Detalle = ex.Message,
                                        IpEquipo = ""
                                    };
                                    db.Logs.Add(log);
                                    await db.SaveChangesAsync();
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch { /* ignore scheduler errors to avoid crashing the host */ }
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}
