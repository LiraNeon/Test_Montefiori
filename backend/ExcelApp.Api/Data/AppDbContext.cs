using ExcelApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExcelApp.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Registro> Registros => Set<Registro>();
}