using Microsoft.AspNetCore.Mvc;
using ColegioPrivado.Data;
using ColegioPrivado.Models;
using System.Linq;
using System.Text.Json;

public class ComprasController : Controller
{
    private readonly ApplicationDbContext _context;

    public ComprasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 📋 LISTADO
    public IActionResult Index()
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";

        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        var compras = _context.Compras
            .Where(c => c.EmpresaId == empresaId)
            .ToList();

        return View(compras);
    }

    // 🟢 GET: CREAR
    public IActionResult Create()
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";

        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        ViewBag.Proveedores = _context.Proveedores
            .Where(p => p.EmpresaId == empresaId)
            .ToList();

        ViewBag.Productos = _context.Productos
            .Where(p => p.EmpresaId == empresaId)
            .ToList();

        return View();
    }

    [HttpPost]
    public IActionResult Create(Compra compra, string DetalleJson)
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";

        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        if (string.IsNullOrEmpty(DetalleJson))
        {
            ViewBag.Proveedores = _context.Proveedores.Where(p => p.EmpresaId == empresaId).ToList();
            ViewBag.Productos = _context.Productos.Where(p => p.EmpresaId == empresaId).ToList();

            ModelState.AddModelError("", "Debes agregar al menos un producto");
            return View(compra);
        }

        using var transaction = _context.Database.BeginTransaction();

        try
        {
            compra.Fecha = DateTime.Now;
            compra.EmpresaId = empresaId;

            var detalles = JsonSerializer.Deserialize<List<DetalleCompra>>(DetalleJson);

            if (detalles == null || detalles.Count == 0)
            {
                ModelState.AddModelError("", "Debe agregar al menos un producto a la compra");

                ViewBag.Proveedores = _context.Proveedores.Where(p => p.EmpresaId == empresaId).ToList();
                ViewBag.Productos = _context.Productos.Where(p => p.EmpresaId == empresaId).ToList();

                return View(compra);
            }

            decimal total = 0;

foreach (var item in detalles)
{
    if (item.PrecioUnitario <= 0)
    {
        ModelState.AddModelError("", "Precio unitario inválido");
        ViewBag.Proveedores = _context.Proveedores.Where(p => p.EmpresaId == empresaId).ToList();
        ViewBag.Productos = _context.Productos.Where(p => p.EmpresaId == empresaId).ToList();
        return View(compra);
    }

    total += item.PrecioUnitario * item.Cantidad;
}

            compra.Total = total;

            var proveedor = _context.Proveedores.Find(compra.ProveedorId);

            if (proveedor != null && proveedor.CreditoDisponible > 0)
            {
                if (proveedor.CreditoDisponible >= compra.Total)
                {
                    proveedor.CreditoDisponible -= compra.Total;
                    compra.Total = 0;
                }
                else
                {
                    compra.Total -= proveedor.CreditoDisponible;
                    proveedor.CreditoDisponible = 0;
                }
            }

            _context.Compras.Add(compra);
            _context.SaveChanges();

            foreach (var item in detalles)
            {
                item.CompraId = compra.CompraId;
                _context.DetalleCompras.Add(item);

                var producto = _context.Productos.Find(item.ProductoId);
                if (producto != null)
                {
                    producto.Stock += item.Cantidad;
                }
            }

            _context.SaveChanges();
            transaction.Commit();

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            ViewBag.Proveedores = _context.Proveedores.Where(p => p.EmpresaId == empresaId).ToList();
            ViewBag.Productos = _context.Productos.Where(p => p.EmpresaId == empresaId).ToList();

            ModelState.AddModelError("", ex.Message);
            return View(compra);
        }
    }
}