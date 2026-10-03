using ExcelApp.Api.Data;
using ExcelApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExcelApp.Api.Services;

public interface IRegistroRepository
{
    Task<List<Registro>> GetAllAsync();
    Task AddRangeAsync(IEnumerable<Registro> registros);
    Task<Registro?> UpdateAsync(int id, Registro datos);
    Task<bool> DeleteAsync(int id);
}

public class RegistroRepository(AppDbContext db) : IRegistroRepository
{
    public Task<List<Registro>> GetAllAsync() =>
        db.Registros.AsNoTracking().OrderBy(r => r.Id).ToListAsync();

    public async Task AddRangeAsync(IEnumerable<Registro> registros)
    {
        db.Registros.AddRange(registros);
        await db.SaveChangesAsync();
    }

    public async Task<Registro?> UpdateAsync(int id, Registro datos)
    {
        var registro = await db.Registros.FindAsync(id);
        if (registro is null) return null;

        registro.Nombre = datos.Nombre;
        registro.Email = datos.Email;
        registro.Telefono = datos.Telefono;
        registro.Distrito = datos.Distrito;
        registro.Sexo = datos.Sexo;
        await db.SaveChangesAsync();
        return registro;
    }

    public async Task<bool> DeleteAsync(int id) =>
        await db.Registros.Where(r => r.Id == id).ExecuteDeleteAsync() > 0;
}
