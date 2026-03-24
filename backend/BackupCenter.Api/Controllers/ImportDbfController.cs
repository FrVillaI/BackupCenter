using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using BackupCenter.Application.Interfaces;
using System.Threading.Tasks;
using System.IO;

namespace BackupCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportDbfController : ControllerBase
{
    private readonly IDbfImportService _importer;
    private readonly ILogger<ImportDbfController> _logger;

    public ImportDbfController(IDbfImportService importer, ILogger<ImportDbfController> logger)
    {
        _importer = importer;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Import([FromQuery] string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            _logger.LogWarning("Intento de importación con path vacío. Usuario: {User}", User.Identity?.Name);
            return BadRequest(new { status = "error", message = "El parámetro 'path' es obligatorio." });
        }

        if (!System.IO.File.Exists(path))
        {
            _logger.LogWarning("Archivo DBF no encontrado: {Path}. Usuario: {User}", path, User.Identity?.Name);
            return NotFound(new { status = "error", message = $"Archivo no encontrado: {path}" });
        }

        try
        {
            _logger.LogInformation("Importación DBF iniciada. Usuario: {User}, Path: {Path}", User.Identity?.Name, path);
            await _importer.ImportAsync(path);
            _logger.LogInformation("Importación DBF finalizada correctamente. Path: {Path}", path);

            return Ok(new { status = "import_completado", path });
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error durante importación DBF. Path: {Path}, Usuario: {User}", path, User.Identity?.Name);
            return StatusCode(500, new { status = "error", message = "Error durante importación DBF", details = ex.Message });
        }
    }
}