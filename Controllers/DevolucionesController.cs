using Microsoft.AspNetCore.Mvc;
using ColegioPrivado.Data;
using System.Linq;
using System.Text.Json;

public class DevolucionesController : Controller
{
    private readonly ApplicationDbContext _context;

    public DevolucionesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔹 LISTAR
    public IActionResult Index()
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";

        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        var devoluciones = _context.DevolucionCompras
            .Where(d => d.EmpresaId == empresaId)
            .ToList();

        return View(devoluciones);
    }

    // 🔹 CREAR (GET)
    public IActionResult Create()
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";

        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        ViewBag.Compras = _context.Compras
            .Where(c => c.EmpresaId == empresaId)
            .ToList();

        ViewBag.Productos = _context.Productos
            .Where(p => p.EmpresaId == empresaId)
            .ToList();

        return View();
    }

    // 🔹 CREAR (POST)
    [HttpPost]
    public IActionResult Create(DevolucionCompra devolucion, string DetalleJson)
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";

        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;
        devolucion.EmpresaId = empresaId;

        using var transaction = _context.Database.BeginTransaction();

        try
        {
            devolucion.Fecha = DateTime.Now;

            var detalles = JsonSerializer.Deserialize<List<DetalleDevolucion>>(DetalleJson);

            if (detalles == null || detalles.Count == 0)
            {
                ModelState.AddModelError("", "Debes agregar productos");
                CargarCombos();
                return View(devolucion);
            }

            _context.DevolucionCompras.Add(devolucion);
            _context.SaveChanges();

            foreach (var item in detalles)
            {
                item.DevolucionCompraId = devolucion.DevolucionCompraId;
                _context.DetalleDevoluciones.Add(item);
            }

            _context.SaveChanges();
            transaction.Commit();

            return RedirectToAction("Index");
        }
        catch
        {
            transaction.Rollback();
            CargarCombos();
            return View(devolucion);
        }
    }

    private void CargarCombos()
    {
        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        ViewBag.Compras = _context.Compras
            .Where(c => c.EmpresaId == empresaId)
            .ToList();

        ViewBag.Productos = _context.Productos
            .Where(p => p.EmpresaId == empresaId)
            .ToList();
    }
}