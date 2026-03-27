using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BackupCenter.Data;
using Microsoft.EntityFrameworkCore;
using BackupCenter.Application.DTOs;

/// Controlador para la gestión de empresas.
/// Permite consultar, activar/desactivar y configurar backups.
namespace BackupCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresasController : ControllerBase
{
    private readonly BackupCenterDbContext _db;
    public EmpresasController(BackupCenterDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN,GERENTE")]
    /// Obtiene listado de empresas.
    public async Task<IActionResult> GetAll()
    {
        IQueryable<BackupCenter.Domain.Entities.Empresa> query = _db.Empresas.AsNoTracking();
        // Si es GERENTE, limitar a las empresas activas (con check de respaldo permitido)
        if (User.IsInRole("GERENTE"))
        {
            query = query.Where(e => e.Activa);
        }
        var empresas = await query.ToListAsync();
        return Ok(empresas);
    }

    [HttpPut("{id}/toggle-activa")]
    [Authorize(Roles = "ADMIN")]
    /// Activa o desactiva una empresa.
    /// Solo disponible para ADMIN.
    public async Task<IActionResult> ToggleActiva(int id)
    {
        var empresa = await _db.Empresas.FirstOrDefaultAsync(e => e.Id == id);

        if (empresa == null)
            return NotFound();

        empresa.Activa = !empresa.Activa;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            empresa.Id,
            empresa.Nombre,
            empresa.Activa
        });
    }

    [HttpPut("{id}/config-backup")]
    [Authorize(Roles = "ADMIN")]
    /// Configura la frecuencia y hora del backup.
    /// Solo permite valores de frecuencia: 12 o 24 horas.
    public async Task<IActionResult> ConfigBackup(int id, [FromBody] ConfigBackupDto dto)
    {
        var empresa = await _db.Empresas.FindAsync(id);

        if (empresa == null)
            return NotFound();

        if (dto.FrecuenciaHoras != 12 && dto.FrecuenciaHoras != 24)
            return BadRequest("Frecuencia inválida");

        empresa.FrecuenciaHoras = dto.FrecuenciaHoras;
        empresa.HoraProgramada = dto.HoraProgramada;

        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("{id}/config")]
    [Authorize(Roles = "ADMIN")]
    /// Actualiza la configuración de backup usando formato de hora en texto (HH:mm).
    public async Task<IActionResult> UpdateConfig(int id, [FromBody] UpdateEmpresaConfigDto dto)
    {
        var empresa = await _db.Empresas.FindAsync(id);
        if (empresa == null) return NotFound();

        if (!TimeSpan.TryParse(dto.HoraProgramada, out var hora))
        {
            return BadRequest("Hora inválida. Formato esperado: HH:mm");
        }

        empresa.HoraProgramada = hora;

        if (dto.FrecuenciaHoras != 12 && dto.FrecuenciaHoras != 24)
        {
            return BadRequest("Frecuencia inválida. Solo 12 o 24 horas permitidas");
        }


        empresa.FrecuenciaHoras = dto.FrecuenciaHoras;
        empresa.HoraProgramada = hora;

        await _db.SaveChangesAsync();

        return Ok();
    }
}
