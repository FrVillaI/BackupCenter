using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BackupCenter.Data;
using Microsoft.EntityFrameworkCore;

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
}
