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
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BackupCenterDbContext>();

                var empresas = await db.Empresas
                    .AsNoTracking()
                    .Where(e => e.Activa)
                    .ToListAsync(stoppingToken);

                var ahora = DateTime.Now;

                foreach (var e in empresas)
                {
                    var horaProgramada = DateTime.Today.Add(e.HoraProgramada);

                    if (
                        ahora >= horaProgramada &&
                        ahora < horaProgramada.AddMinutes(1)
                    )
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
                                    var log = new LogEntry
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
            catch
            {
                // evitar caída del servicio
            }

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}
