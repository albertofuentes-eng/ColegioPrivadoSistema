using Microsoft.AspNetCore.Mvc;
using ColegioPrivado.Data;
using ColegioPrivado.Models;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

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
            .Include(c => c.Proveedor)
            .Include(c => c.Detalles!)
                .ThenInclude(d => d.Producto)
            .OrderByDescending(c => c.CompraId)
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

        // Validar que el proveedor exista
        if (compra.ProveedorId <= 0)
        {
            ViewBag.Proveedores = _context.Proveedores.Where(p => p.EmpresaId == empresaId).ToList();
            ViewBag.Productos = _context.Productos.Where(p => p.EmpresaId == empresaId).ToList();
            ModelState.AddModelError("", "Debe seleccionar un proveedor válido");
            return View(compra);
        }

        var proveedorExiste = _context.Proveedores.Any(p => p.ProveedorId == compra.ProveedorId && p.EmpresaId == empresaId);
        if (!proveedorExiste)
        {
            ViewBag.Proveedores = _context.Proveedores.Where(p => p.EmpresaId == empresaId).ToList();
            ViewBag.Productos = _context.Productos.Where(p => p.EmpresaId == empresaId).ToList();
            ModelState.AddModelError("", "El proveedor seleccionado no existe");
            return View(compra);
        }

        if (string.IsNullOrEmpty(DetalleJson) || DetalleJson.Trim() == "[]")
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
                if (item.ProductoId <= 0)
                {
                    ModelState.AddModelError("", "Producto inválido en los detalles");
                    ViewBag.Proveedores = _context.Proveedores.Where(p => p.EmpresaId == empresaId).ToList();
                    ViewBag.Productos = _context.Productos.Where(p => p.EmpresaId == empresaId).ToList();
                    return View(compra);
                }

                if (item.Cantidad <= 0)
                {
                    ModelState.AddModelError("", "Cantidad inválida");
                    ViewBag.Proveedores = _context.Proveedores.Where(p => p.EmpresaId == empresaId).ToList();
                    ViewBag.Productos = _context.Productos.Where(p => p.EmpresaId == empresaId).ToList();
                    return View(compra);
                }

                if (item.PrecioUnitario <= 0)
                {
                    ModelState.AddModelError("", "Precio unitario inválido");
                    ViewBag.Proveedores = _context.Proveedores.Where(p => p.EmpresaId == empresaId).ToList();
                    ViewBag.Productos = _context.Productos.Where(p => p.EmpresaId == empresaId).ToList();
                    return View(compra);
                }

                total += item.PrecioUnitario * item.Cantidad;
            }

            compra.Total = total + compra.CostoEnvio;

            var proveedor = _context.Proveedores.Find(compra.ProveedorId);
            if (proveedor == null)
            {
                ModelState.AddModelError("", "El proveedor no existe");
                ViewBag.Proveedores = _context.Proveedores.Where(p => p.EmpresaId == empresaId).ToList();
                ViewBag.Productos = _context.Productos.Where(p => p.EmpresaId == empresaId).ToList();
                transaction.Rollback();
                return View(compra);
            }

            // Aplicar crédito disponible del proveedor
            decimal totalAntesDeCredito = compra.Total;
            if (proveedor.CreditoDisponible > 0)
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
                
                // Validar que el producto exista
                var producto = _context.Productos.FirstOrDefault(p => p.ProductoId == item.ProductoId && p.EmpresaId == empresaId);
                if (producto == null)
                {
                    throw new Exception($"El producto con ID {item.ProductoId} no existe");
                }

                // Generar NumeroLote único por línea de compra
                item.NumeroLote = $"LOTE-{compra.ProveedorId}-{compra.CompraId}-{DateTime.Now.Ticks}";
                
                _context.DetalleCompras.Add(item);

                // Crear registro de lote para trazabilidad
                var loteCompra = new Lote
                {
                    ProductoId = item.ProductoId,
                    ProveedorId = compra.ProveedorId,
                    NumeroLote = item.NumeroLote,
                    PrecioCompra = item.PrecioUnitario,
                    CantidadComprada = item.Cantidad,
                    FechaCompra = compra.Fecha,
                    EmpresaId = empresaId
                };
                _context.Lotes.Add(loteCompra);

                // Actualizar stock del producto (global, no por lote)
                producto.Stock += item.Cantidad;
                producto.CodigoLote = "Lote-" + DateTime.Now.Ticks;
                
                // Crear registro de producto pendiente para rastreo
                var productoPendiente = new ProductoPendiente
                {
                    ProductoId = item.ProductoId,
                    CompraId = compra.CompraId,
                    Cantidad = item.Cantidad,
                    CantidadProcesada = 0,
                    FechaCompra = compra.Fecha,
                    Estado = "Pendiente",
                    EmpresaId = empresaId
                };
                _context.ProductosPendientes.Add(productoPendiente);
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