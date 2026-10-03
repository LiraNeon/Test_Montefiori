using System.Text;
using ExcelApp.Api.Services;
using ExcelApp.Tests.Helpers;

namespace ExcelApp.Tests;

public class ExcelParserTests
{
    private readonly ExcelParser _parser = new();

    [Fact]
    public void Parse_ArchivoValido_DevuelveTodosLosRegistros()
    {
        using var excel = TestHelpers.CrearExcel(
            ["Nombre", "Email", "Telefono", "Distrito", "Sexo"],
            ["Pepe", "pepe@gmail.com", "999878797", "Ate", "H"],
            ["Ana", "ana@gmail.com", "999444120", "La Molina", "M"]);

        var registros = _parser.Parse(excel);

        Assert.Equal(2, registros.Count);
        Assert.Equal("Pepe", registros[0].Nombre);
        Assert.Equal("pepe@gmail.com", registros[0].Email);
        Assert.Equal("999878797", registros[0].Telefono);
        Assert.Equal("Ate", registros[0].Distrito);
        Assert.Equal("H", registros[0].Sexo);
    }

    [Fact]
    public void Parse_EncabezadosEnMayusculasYConEspacios_LosReconoce()
    {
        using var excel = TestHelpers.CrearExcel(
            [" NOMBRE ", "email"],
            ["Pepe", "pepe@gmail.com"]);

        Assert.Single(_parser.Parse(excel));
    }

    [Fact]
    public void Parse_SinColumnasOpcionales_LasDejaEnNull()
    {
        using var excel = TestHelpers.CrearExcel(
            ["Nombre", "Email"],
            ["Pepe", "pepe@gmail.com"]);

        var registro = Assert.Single(_parser.Parse(excel));

        Assert.Null(registro.Telefono);
        Assert.Null(registro.Distrito);
        Assert.Null(registro.Sexo);
    }

    [Fact]
    public void Parse_SexoEnMinuscula_LoGuardaEnMayuscula()
    {
        using var excel = TestHelpers.CrearExcel(
            ["Nombre", "Email", "Sexo"],
            ["Ana", "ana@gmail.com", "m"]);

        var registro = Assert.Single(_parser.Parse(excel));

        Assert.Equal("M", registro.Sexo);
    }

    [Fact]
    public void Parse_FilaSinNombreNiEmail_LaIgnora()
    {
        using var excel = TestHelpers.CrearExcel(
            ["Nombre", "Email", "Distrito"],
            ["Pepe", "pepe@gmail.com", "Ate"],
            ["", "", "Comas"],
            ["Ana", "ana@gmail.com", "Ate"]);

        Assert.Equal(2, _parser.Parse(excel).Count);
    }

    [Fact]
    public void Parse_FaltaColumnaEmail_LanzaErrorConMensaje()
    {
        using var excel = TestHelpers.CrearExcel(
            ["Nombre", "Telefono"],
            ["Pepe", "999878797"]);

        var ex = Assert.Throws<InvalidDataException>(() => _parser.Parse(excel));

        Assert.Contains("email", ex.Message);
    }

    [Fact]
    public void Parse_ArchivoQueNoEsExcel_LanzaError()
    {
        using var noEsExcel = new MemoryStream(Encoding.UTF8.GetBytes("esto no es un excel"));

        var ex = Assert.Throws<InvalidDataException>(() => _parser.Parse(noEsExcel));

        Assert.Equal("El archivo no es un Excel válido.", ex.Message);
    }
}
