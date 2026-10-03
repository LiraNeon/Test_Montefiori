using ClosedXML.Excel;
using ExcelApp.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExcelApp.Tests.Helpers;

public static class TestHelpers
{
    // Genera un Excel en memoria con los encabezados y filas indicados
    public static MemoryStream CrearExcel(string[] encabezados, params string[][] filas)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Datos");

        for (int c = 0; c < encabezados.Length; c++)
            ws.Cell(1, c + 1).Value = encabezados[c];

        for (int f = 0; f < filas.Length; f++)
            for (int c = 0; c < filas[f].Length; c++)
                ws.Cell(f + 2, c + 1).Value = filas[f][c];

        var stream = new MemoryStream();
        wb.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    // Crea un AppDbContext sobre SQLite en memoria; la base existe mientras la conexión siga abierta
    public static AppDbContext CrearDb()
    {
        var conexion = new SqliteConnection("Data Source=:memory:");
        conexion.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(conexion)
            .Options;

        var db = new AppDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }
}
