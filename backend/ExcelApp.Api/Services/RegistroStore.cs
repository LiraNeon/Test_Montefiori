using ExcelApp.Api.Models;

namespace ExcelApp.Api.Services;

public class RegistroStore
{
    private readonly List<Registro> _items = [];
    private int _nextId = 1;
    private readonly object _lock = new();

    public List<Registro> GetAll() { lock (_lock) return _items.ToList(); }

    public void AddRange(IEnumerable<Registro> registros)
    {
        lock (_lock)
            foreach (var r in registros) { r.Id = _nextId++; _items.Add(r); }
    }

    public bool Delete(int id) { lock (_lock) return _items.RemoveAll(r => r.Id == id) > 0; }
}