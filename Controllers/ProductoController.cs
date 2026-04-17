using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ColegioPrivado.Data;
using ColegioPrivado.Models;

public class ProductoController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductoController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        int empresaId = 1;

        var productos = _context.Productos
            .Where(p => p.EmpresaId == empresaId)
            .ToList();

        return View(productos);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Producto producto)
    {
        int empresaId = 1;

        if (!ModelState.IsValid)
            return View(producto);

        producto.EmpresaId = empresaId;

        _context.Productos.Add(producto);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var producto = _context.Productos.Find(id);
        if (producto == null) return NotFound();

        return View(producto);
    }

    [HttpPost]
    public IActionResult Edit(Producto producto)
    {
        if (!ModelState.IsValid)
            return View(producto);

        _context.Productos.Update(producto);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var producto = _context.Productos.Find(id);
        if (producto != null)
        {
            _context.Productos.Remove(producto);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}