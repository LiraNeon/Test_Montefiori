using ExcelApp.Api.Models;
using ExcelApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExcelApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrosController(IRegistroRepository repo, IExcelParser parser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Registro>>> GetAll() => await repo.GetAllAsync();

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile? archivo)
    {
        if (archivo is null || archivo.Length == 0)
            return BadRequest(new { mensaje = "Debe seleccionar un archivo." });

        if (!Path.GetExtension(archivo.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { mensaje = "Solo se permiten archivos .xlsx." });

        try
        {
            using var stream = archivo.OpenReadStream();
            var registros = parser.Parse(stream);
            if (registros.Count == 0)
                return BadRequest(new { mensaje = "El archivo no contiene registros." });

            await repo.AddRangeAsync(registros);
            return Ok(new { insertados = registros.Count });
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await repo.DeleteAsync(id) ? NoContent() : NotFound(new { mensaje = $"El registro {id} no existe." });
}
