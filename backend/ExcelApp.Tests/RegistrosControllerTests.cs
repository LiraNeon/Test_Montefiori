using System.Text;
using ExcelApp.Api.Controllers;
using ExcelApp.Api.Data;
using ExcelApp.Api.Models;
using ExcelApp.Api.Services;
using ExcelApp.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExcelApp.Tests;

public class RegistrosControllerTests : IDisposable
{
    private readonly AppDbContext _db = TestHelpers.CrearDb();
    private readonly RegistroRepository _repo;
    private readonly RegistrosController _controller;

    public RegistrosControllerTests()
    {
        _repo = new RegistroRepository(_db);
        _controller = new RegistrosController(_repo, new ExcelParser());
    }

    public void Dispose() => _db.Dispose();

    private static IFormFile Archivo(Stream contenido, string nombre) =>
        new FormFile(contenido, 0, contenido.Length, "archivo", nombre);

    // Lee la propiedad "mensaje" del objeto anónimo que devuelve el controlador
    private static string? Mensaje(IActionResult result)
    {
        var valor = ((ObjectResult)result).Value!;
        return valor.GetType().GetProperty("mensaje")?.GetValue(valor) as string;
    }

    [Fact]
    public async Task Upload_ExcelValido_InsertaRegistros()
    {
        using var excel = TestHelpers.CrearExcel(
            ["Nombre", "Email"],
            ["Pepe", "pepe@gmail.com"],
            ["Ana", "ana@gmail.com"]);

        var result = await _controller.Upload(Archivo(excel, "datos.xlsx"));

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(2, (await _repo.GetAllAsync()).Count);
    }

    [Fact]
    public async Task Upload_SinArchivo_Devuelve400()
    {
        var result = await _controller.Upload(null);

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Debe seleccionar un archivo.", Mensaje(result));
    }

    [Fact]
    public async Task Upload_ExtensionIncorrecta_Devuelve400()
    {
        using var contenido = new MemoryStream(Encoding.UTF8.GetBytes("a,b"));

        var result = await _controller.Upload(Archivo(contenido, "datos.csv"));

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Solo se permiten archivos .xlsx.", Mensaje(result));
    }

    [Fact]
    public async Task Upload_ExcelSinColumnasRequeridas_Devuelve400YNoInserta()
    {
        using var excel = TestHelpers.CrearExcel(["Nombre"], ["Pepe"]);

        var result = await _controller.Upload(Archivo(excel, "datos.xlsx"));

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(await _repo.GetAllAsync());
    }

    [Fact]
    public async Task Update_SinNombre_Devuelve400()
    {
        var result = await _controller.Update(1, new Registro { Nombre = "", Email = "a@gmail.com" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_RegistroInexistente_Devuelve404()
    {
        var result = await _controller.Update(999, new Registro { Nombre = "A", Email = "a@gmail.com" });

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_RegistroExistente_Devuelve204()
    {
        await _repo.AddRangeAsync([new Registro { Nombre = "Pepe", Email = "pepe@gmail.com" }]);
        var id = (await _repo.GetAllAsync())[0].Id;

        var result = await _controller.Delete(id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_RegistroInexistente_Devuelve404()
    {
        var result = await _controller.Delete(999);

        Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("El registro 999 no existe.", Mensaje(result));
    }
}
