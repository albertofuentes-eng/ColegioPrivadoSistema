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

    // 🔹 LISTAR
    public IActionResult Index()
    {
        var productos = _context.Productos.ToList();
        return View(productos);
    }

    // 🔹 GET: CREAR
    public IActionResult Create()
    {
        return View();
    }

    // 🔹 POST: CREAR
    [HttpPost]
    public IActionResult Create(Producto producto)
    {
        if (!ModelState.IsValid)
            return View(producto);

        _context.Productos.Add(producto);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // 🔹 GET: EDITAR
    public IActionResult Edit(int id)
    {
        var producto = _context.Productos.Find(id);
        if (producto == null) return NotFound();

        return View(producto);
    }

    // 🔹 POST: EDITAR
    [HttpPost]
    public IActionResult Edit(Producto producto)
    {
        if (!ModelState.IsValid)
            return View(producto);

        _context.Productos.Update(producto);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // 🔹 ELIMINAR
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