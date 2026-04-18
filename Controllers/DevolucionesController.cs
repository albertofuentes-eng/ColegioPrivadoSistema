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

            // 🆕 Usar el validador centralizado
            var validador = new ValidadorDevoluciones(_context);
            var (esValido, errores) = validador.Validar(devolucion.CompraId, detalles, devolucion.EsCredito);

            if (!esValido)
            {
                foreach (var error in errores)
                {
                    ModelState.AddModelError("", error);
                }
                CargarCombos();
                return View(devolucion);
            }

            var compra = _context.Compras.Find(devolucion.CompraId);
            if (compra == null)
            {
                ModelState.AddModelError("", "Compra relacionada no encontrada.");
                CargarCombos();
                return View(devolucion);
            }

            _context.DevolucionCompras.Add(devolucion);
            _context.SaveChanges();

            if (devolucion.EsCredito)
            {
                var proveedor = _context.Proveedores.Find(compra.ProveedorId);
                if (proveedor != null)
                {
                    proveedor.CreditoDisponible += devolucion.TotalCredito;
                }
            }

            foreach (var item in detalles)
            {
                item.DevolucionCompraId = devolucion.DevolucionCompraId;
                _context.DetalleDevoluciones.Add(item);

                if (!devolucion.EsCredito && item.Danado)
                {
                    var producto = _context.Productos.Find(item.ProductoId);
                    if (producto != null)
                    {
                        // 🆕 Reducir stock cuando se devuelve producto dañado
                        producto.Stock -= item.Cantidad;
                        if (producto.Stock < 0) producto.Stock = 0;
                    }
                    
                    // 🆕 Actualizar ProductosPendiente cuando se devuelve
                    var productoPendiente = _context.ProductosPendientes
                        .FirstOrDefault(pp => pp.CompraId == devolucion.CompraId 
                            && pp.ProductoId == item.ProductoId 
                            && pp.Estado == "Pendiente");
                    
                    if (productoPendiente != null)
                    {
                        productoPendiente.CantidadProcesada += item.Cantidad;
                        if (productoPendiente.CantidadProcesada >= productoPendiente.Cantidad)
                        {
                            productoPendiente.Estado = "Procesado";
                        }
                    }
                }
                else if (devolucion.EsCredito)
                {
                    // 🆕 Registrar devolución por crédito en ProductosPendiente
                    var productoPendiente = _context.ProductosPendientes
                        .FirstOrDefault(pp => pp.CompraId == devolucion.CompraId 
                            && pp.ProductoId == item.ProductoId);
                    
                    if (productoPendiente != null)
                    {
                        productoPendiente.CantidadProcesada += item.Cantidad;
                        if (productoPendiente.CantidadProcesada >= productoPendiente.Cantidad)
                        {
                            productoPendiente.Estado = "Cancelado";
                        }
                    }
                }
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

    // 🆕 VER CRÉDITOS DISPONIBLES
    public IActionResult CreditosDisponibles()
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";

        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        var proveedores = _context.Proveedores
            .Where(p => p.EmpresaId == empresaId)
            .OrderByDescending(p => p.CreditoDisponible)
            .ToList();

        return View(proveedores);
    }
}