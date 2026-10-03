using ExcelApp.Api.Data;
using ExcelApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExcelApp.Api.Services;

public interface IRegistroRepository
{
    Task<List<Registro>> GetAllAsync();
    Task AddRangeAsync(IEnumerable<Registro> registros);
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

    public async Task<bool> DeleteAsync(int id) =>
        await db.Registros.Where(r => r.Id == id).ExecuteDeleteAsync() > 0;
}
