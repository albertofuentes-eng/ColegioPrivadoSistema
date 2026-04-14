using Microsoft.AspNetCore.Mvc;
using ColegioPrivado.Data;
using ColegioPrivado.Models;
using System.Linq;

public class ProveedoresController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProveedoresController(ApplicationDbContext context)
    {
        _context = context;
    }

    // LISTA
    public IActionResult Index()
    {
        var proveedores = _context.Proveedores.ToList();
        return View(proveedores);
    }

    // CREAR (GET)
    public IActionResult Create()
    {
        return View();
    }

    // CREAR (POST)
    [HttpPost]
    public IActionResult Create(Proveedor proveedor)
    {
        if (!ModelState.IsValid)
            return View(proveedor);

        proveedor.Activo = true;

        _context.Proveedores.Add(proveedor);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // EDITAR (GET)
    public IActionResult Edit(int id)
    {
        var proveedor = _context.Proveedores.Find(id);

        if (proveedor == null)
            return NotFound();

        return View(proveedor);
    }

    // EDITAR (POST)
    [HttpPost]
    public IActionResult Edit(Proveedor proveedor)
    {
        if (!ModelState.IsValid)
            return View(proveedor);

        _context.Proveedores.Update(proveedor);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // ELIMINAR (lógico)
    public IActionResult Delete(int id)
    {
        var proveedor = _context.Proveedores.Find(id);

        if (proveedor != null)
        {
            proveedor.Activo = false;
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}