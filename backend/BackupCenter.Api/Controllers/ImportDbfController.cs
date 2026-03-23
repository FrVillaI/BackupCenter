using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BackupCenter.Application.Interfaces;
using System.Threading.Tasks;

namespace BackupCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportDbfController : ControllerBase
{
    private readonly IDbfImportService _importer;
    public ImportDbfController(IDbfImportService importer)
    {
        _importer = importer;
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Import([FromQuery] string? path)
    {
        var p = path ?? string.Empty;
        // Llamada al servicio de importación DBF (placeholder para MVP)
        await _importer.ImportAsync(p);
        return Ok(new { status = "import_iniciado", path = p });
    }
}
