using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ColegioPrivado.Data;
using ColegioPrivado.Models;
using Microsoft.EntityFrameworkCore;

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
        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        if (!ModelState.IsValid)
            return View(producto);

        producto.EmpresaId = empresaId;

        // Generar lote inicial para el producto
        string numeroLoteInicial = $"LOTE-INICIAL-{producto.ProductoId}-{DateTime.Now.Ticks}";
        producto.CodigoLote = numeroLoteInicial;

        _context.Productos.Add(producto);
        _context.SaveChanges();

        // Crear lote inicial (cantidad 0, precio 0)
        var loteInicial = new Lote
        {
            ProductoId = producto.ProductoId,
            ProveedorId = 0, // Proveedor inicial (ninguno)
            NumeroLote = numeroLoteInicial,
            PrecioCompra = 0,
            CantidadComprada = 0,
            FechaCompra = DateTime.Now,
            EmpresaId = empresaId
        };

        _context.Lotes.Add(loteInicial);
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

    public IActionResult Lotes(int id)
    {
        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        var producto = _context.Productos
            .FirstOrDefault(p => p.ProductoId == id && p.EmpresaId == empresaId);

        if (producto == null)
            return NotFound();

        var lotes = _context.Lotes
            .Where(l => l.ProductoId == id && l.EmpresaId == empresaId)
            .Include(l => l.Proveedor)
            .OrderByDescending(l => l.FechaCompra)
            .ToList();

        ViewBag.Producto = producto;
        return View(lotes);
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