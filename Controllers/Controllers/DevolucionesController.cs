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
        var devoluciones = _context.DevolucionCompras.ToList();
        return View(devoluciones);
    }

    // 🔹 CREAR (GET)
    public IActionResult Create()
    {
        ViewBag.Compras = _context.Compras.ToList();
        ViewBag.Productos = _context.Productos.ToList();

        return View();
    }

    // 🔹 CREAR (POST)
    [HttpPost]
    public IActionResult Create(DevolucionCompra devolucion, string DetalleJson)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            devolucion.Fecha = DateTime.Now;

            var detalles = JsonSerializer.Deserialize<List<DetalleDevolucion>>(DetalleJson);

            if (detalles == null || detalles.Count == 0)
            {
                ModelState.AddModelError("", "Debes agregar productos");

                ViewBag.Compras = _context.Compras.ToList();
                ViewBag.Productos = _context.Productos.ToList();

                return View(devolucion);
            }

            // 🔥 VALIDAR DÍAS
            var compra = _context.Compras.Find(devolucion.CompraId);

            if (compra != null)
            {
                var dias = (DateTime.Now - compra.Fecha).TotalDays;

                if (dias > 7)
                {
                    ModelState.AddModelError("", "Tiempo de devolución expirado");

                    ViewBag.Compras = _context.Compras.ToList();
                    ViewBag.Productos = _context.Productos.ToList();

                    return View(devolucion);
                }
            }

            // 🔥 VALIDACIONES POR PRODUCTO
            foreach (var item in detalles)
            {
                // ❌ NO negativos
                if (item.Cantidad <= 0)
                {
                    ModelState.AddModelError("", "Cantidad inválida");

                    ViewBag.Compras = _context.Compras.ToList();
                    ViewBag.Productos = _context.Productos.ToList();

                    return View(devolucion);
                }

                var producto = _context.Productos.Find(item.ProductoId);

                if (producto == null)
                {
                    ModelState.AddModelError("", "Producto no existe");

                    ViewBag.Compras = _context.Compras.ToList();
                    ViewBag.Productos = _context.Productos.ToList();

                    return View(devolucion);
                }

                // 🚫 Producto en oferta
                if (producto.EnOferta)
                {
                    ModelState.AddModelError("", "No se puede devolver producto en oferta");

                    ViewBag.Compras = _context.Compras.ToList();
                    ViewBag.Productos = _context.Productos.ToList();

                    return View(devolucion);
                }

                // 💀 VALIDAR QUE EL PRODUCTO PERTENECE A LA COMPRA
                var detalleCompra = _context.DetalleCompras
                    .FirstOrDefault(d => d.CompraId == devolucion.CompraId
                                      && d.ProductoId == item.ProductoId);

                if (detalleCompra == null)
                {
                    ModelState.AddModelError("", "Producto no pertenece a la compra");

                    ViewBag.Compras = _context.Compras.ToList();
                    ViewBag.Productos = _context.Productos.ToList();

                    return View(devolucion);
                }

                // 💀 SUMAR LO YA DEVUELTO
                var devolucionesIds = _context.DevolucionCompras
                    .Where(dc => dc.CompraId == devolucion.CompraId)
                    .Select(dc => dc.DevolucionCompraId)
                    .ToList();

                var devuelto = _context.DetalleDevoluciones
                    .Where(d => d.ProductoId == item.ProductoId
                             && devolucionesIds.Contains(d.DevolucionCompraId))
                    .Sum(d => (int?)d.Cantidad) ?? 0;

                // 💀 VALIDAR LÍMITE
                if ((devuelto + item.Cantidad) > detalleCompra.Cantidad)
                {
                    ModelState.AddModelError("", "No puedes devolver más de lo comprado");

                    ViewBag.Compras = _context.Compras.ToList();
                    ViewBag.Productos = _context.Productos.ToList();

                    return View(devolucion);
                }
            }

            //////////////////////////////////////////////////////
            // 🔥 TODO VALIDADO → AHORA SÍ GUARDAR
            //////////////////////////////////////////////////////

            _context.DevolucionCompras.Add(devolucion);
            _context.SaveChanges();

            foreach (var item in detalles)
{
    item.DevolucionCompraId = devolucion.DevolucionCompraId;

    _context.DetalleDevoluciones.Add(item);

    var producto = _context.Productos.Find(item.ProductoId);
    if (producto != null)
    {
        if (!devolucion.EsCredito)
        {
            producto.Stock -= item.Cantidad;
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

            ViewBag.Compras = _context.Compras.ToList();
            ViewBag.Productos = _context.Productos.ToList();

            return View(devolucion);
        }
    }
}