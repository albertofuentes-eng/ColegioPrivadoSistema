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

    public IActionResult Index()
    {
        int empresaId = 1;

        var proveedores = _context.Proveedores
            .Where(p => p.EmpresaId == empresaId)
            .ToList();

        return View(proveedores);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Proveedor proveedor)
    {
        int empresaId = 1;

        if (!ModelState.IsValid)
            return View(proveedor);

        proveedor.Activo = true;
        proveedor.EmpresaId = empresaId;

        _context.Proveedores.Add(proveedor);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var proveedor = _context.Proveedores.Find(id);
        if (proveedor == null) return NotFound();

        return View(proveedor);
    }

    [HttpPost]
    public IActionResult Edit(Proveedor proveedor)
    {
        if (!ModelState.IsValid)
            return View(proveedor);

        _context.Proveedores.Update(proveedor);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

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