using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BackupCenter.Application.Interfaces;
using System.Threading.Tasks;
using BackupCenter.Data;

/// Controlador para gestión de backups.
/// Permite iniciar backups manuales.
namespace BackupCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BackupsController : ControllerBase
{
    private readonly IBackupService _backup;
    private readonly BackupCenterDbContext _db;
    public BackupsController(IBackupService backup, BackupCenterDbContext db)
    {
        _backup = backup;
        _db = db;
    }

    [HttpPost("{empresaId}")]
    [Authorize(Roles = "ADMIN,GERENTE")]
    public async Task<IActionResult> StartBackup(int empresaId, [FromQuery] string? overridePath = null)
    {
        /// Ejecuta un backup manual para una empresa.
        if (User.IsInRole("GERENTE"))
        {
            var emp = await _db.Empresas.FindAsync(empresaId);
            if (emp == null || !emp.Activa)
            {
                return Forbid();
            }
        }
        try
        {
            var result = await _backup.CreateBackupAsync(empresaId, overridePath, isAutomatic: false);
            return Ok(new { status = "backup_iniciado", empresaId, zip = result.ZipPath, hash = result.Hash, hashPath = result.HashPath });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
