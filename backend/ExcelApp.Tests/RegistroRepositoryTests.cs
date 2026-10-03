using ExcelApp.Api.Data;
using ExcelApp.Api.Models;
using ExcelApp.Api.Services;
using ExcelApp.Tests.Helpers;

namespace ExcelApp.Tests;

public class RegistroRepositoryTests : IDisposable
{
    private readonly AppDbContext _db = TestHelpers.CrearDb();
    private readonly RegistroRepository _repo;

    public RegistroRepositoryTests() => _repo = new RegistroRepository(_db);

    public void Dispose() => _db.Dispose();

    private static Registro Nuevo(string nombre) => new() { Nombre = nombre, Email = $"{nombre}@gmail.com" };

    [Fact]
    public async Task AddRange_AsignaIdsYLosGuarda()
    {
        await _repo.AddRangeAsync([Nuevo("Pepe"), Nuevo("Ana")]);

        var todos = await _repo.GetAllAsync();

        Assert.Equal(2, todos.Count);
        Assert.All(todos, r => Assert.True(r.Id > 0));
    }

    [Fact]
    public async Task GetAll_BaseVacia_DevuelveListaVacia()
    {
        Assert.Empty(await _repo.GetAllAsync());
    }

    [Fact]
    public async Task Update_RegistroExistente_ModificaLosCampos()
    {
        await _repo.AddRangeAsync([Nuevo("Pepe")]);
        var id = (await _repo.GetAllAsync())[0].Id;

        var actualizado = await _repo.UpdateAsync(id, new Registro
        {
            Nombre = "Pepe Editado",
            Email = "nuevo@gmail.com",
            Distrito = "Lince",
            Sexo = "H"
        });

        Assert.NotNull(actualizado);
        var guardado = (await _repo.GetAllAsync())[0];
        Assert.Equal("Pepe Editado", guardado.Nombre);
        Assert.Equal("nuevo@gmail.com", guardado.Email);
        Assert.Equal("Lince", guardado.Distrito);
    }

    [Fact]
    public async Task Update_RegistroInexistente_DevuelveNull()
    {
        Assert.Null(await _repo.UpdateAsync(999, Nuevo("X")));
    }

    [Fact]
    public async Task Delete_RegistroExistente_LoElimina()
    {
        await _repo.AddRangeAsync([Nuevo("Pepe"), Nuevo("Ana")]);
        var id = (await _repo.GetAllAsync())[0].Id;

        Assert.True(await _repo.DeleteAsync(id));

        var restantes = await _repo.GetAllAsync();
        Assert.Single(restantes);
        Assert.DoesNotContain(restantes, r => r.Id == id);
    }

    [Fact]
    public async Task Delete_RegistroInexistente_DevuelveFalse()
    {
        Assert.False(await _repo.DeleteAsync(999));
    }
}
