using ClosedXML.Excel;
using ExcelApp.Api.Models;

namespace ExcelApp.Api.Services;

public interface IExcelParser
{
    List<Registro> Parse(Stream stream);
}

public class ExcelParser : IExcelParser
{
    private static readonly string[] ColumnasRequeridas = ["nombre", "email"];

    public List<Registro> Parse(Stream stream)
    {
        XLWorkbook wb;
        try { wb = new XLWorkbook(stream); }
        catch (Exception) { throw new InvalidDataException("El archivo no es un Excel válido."); }

        using (wb)
        {
            var ws = wb.Worksheets.First();
            var header = ws.FirstRowUsed() ?? throw new InvalidDataException("El archivo está vacío.");

            var columnas = header.CellsUsed()
                .ToDictionary(c => c.GetString().Trim().ToLowerInvariant(), c => c.Address.ColumnNumber);

            var faltantes = ColumnasRequeridas.Where(c => !columnas.ContainsKey(c)).ToList();
            if (faltantes.Count > 0)
                throw new InvalidDataException($"Faltan columnas: {string.Join(", ", faltantes)}");

            var registros = new List<Registro>();
            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var nombre = row.Cell(columnas["nombre"]).GetString().Trim();
                var email = row.Cell(columnas["email"]).GetString().Trim();
                if (nombre == "" && email == "") continue; // fila vacía

                registros.Add(new Registro
                {
                    Nombre = nombre,
                    Email = email,
                    Telefono = Opcional(row, "telefono"),
                    Distrito = Opcional(row, "distrito"),
                    Sexo = Opcional(row, "sexo")?.ToUpperInvariant()
                });
            }
            return registros;

            // Lee una columna que puede no venir en el Excel
            string? Opcional(IXLRow row, string columna) =>
                columnas.TryGetValue(columna, out var col) ? row.Cell(col).GetString().Trim() : null;
        }
    }
}