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
        var compras = _context.Compras.ToList();
        return View(compras);
    }

    // 🟢 GET: CREAR
    public IActionResult Create()
    {
        ViewBag.Proveedores = _context.Proveedores.ToList();
        ViewBag.Productos = _context.Productos.ToList();

        return View();
    }

    // 🔵 POST: GUARDAR COMPRA
    [HttpPost]
    public IActionResult Create(Compra compra, string DetalleJson)
    {
        // 🔥 VALIDAR SI NO VIENE NADA
        if (string.IsNullOrEmpty(DetalleJson))
        {
            ViewBag.Proveedores = _context.Proveedores.ToList();
            ViewBag.Productos = _context.Productos.ToList();

            ModelState.AddModelError("", "Debes agregar al menos un producto");

            return View(compra);
        }

        using var transaction = _context.Database.BeginTransaction();

        try
        {
            compra.Fecha = DateTime.Now;

            _context.Compras.Add(compra);
            _context.SaveChanges();

            var detalles = JsonSerializer.Deserialize<List<DetalleCompra>>(DetalleJson);

            // 🔥 VALIDACIÓN PRO
            if (detalles == null || detalles.Count == 0)
            {
                ModelState.AddModelError("", "Debe agregar al menos un producto a la compra");

                ViewBag.Proveedores = _context.Proveedores.ToList();
                ViewBag.Productos = _context.Productos.ToList();

                return View(compra);
            }

            foreach (var item in detalles)
            {
                item.CompraId = compra.CompraId;

                _context.DetalleCompras.Add(item);

                // 🔥 ACTUALIZA STOCK
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

            ViewBag.Proveedores = _context.Proveedores.ToList();
            ViewBag.Productos = _context.Productos.ToList();

            // 🔥 MOSTRAR ERROR REAL
            ModelState.AddModelError("", ex.Message);

            return View(compra);
        }
    }
}