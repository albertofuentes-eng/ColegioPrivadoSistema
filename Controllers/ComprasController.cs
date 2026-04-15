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

    [HttpPost]
public IActionResult Create(Compra compra, string DetalleJson)
{
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

        var detalles = JsonSerializer.Deserialize<List<DetalleCompra>>(DetalleJson);

        if (detalles == null || detalles.Count == 0)
        {
            ModelState.AddModelError("", "Debe agregar al menos un producto a la compra");

            ViewBag.Proveedores = _context.Proveedores.ToList();
            ViewBag.Productos = _context.Productos.ToList();

            return View(compra);
        }

        //////////////////////////////////////////////////////
        // 🔥 CALCULAR TOTAL DE LA COMPRA
        //////////////////////////////////////////////////////
        decimal total = 0;

        foreach (var item in detalles)
        {
            var producto = _context.Productos.Find(item.ProductoId);

            if (producto != null)
            {
                total += producto.PrecioCompra * item.Cantidad;
            }
        }

        compra.Total = total;

        //////////////////////////////////////////////////////
        // 🔥 APLICAR CRÉDITO DEL PROVEEDOR (AQUÍ VA LO NUEVO)
        //////////////////////////////////////////////////////
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

        //////////////////////////////////////////////////////
        // 🔥 GUARDAR COMPRA
        //////////////////////////////////////////////////////
        _context.Compras.Add(compra);
        _context.SaveChanges();

        //////////////////////////////////////////////////////
        // 🔥 GUARDAR DETALLES + STOCK
        //////////////////////////////////////////////////////
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

        ViewBag.Proveedores = _context.Proveedores.ToList();
        ViewBag.Productos = _context.Productos.ToList();

        ModelState.AddModelError("", ex.Message);

        return View(compra);
    }
}
}