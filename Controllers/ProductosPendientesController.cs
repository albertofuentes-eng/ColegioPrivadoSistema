using Microsoft.AspNetCore.Mvc;
using ColegioPrivado.Data;
using System.Linq;

/// <summary>
/// Controlador para gestionar ProductosPendientes
/// Permite visualizar y actualizar el estado de productos pendientes de procesar
/// </summary>
public class ProductosPendientesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductosPendientesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 📋 LISTADO
    public IActionResult Index(string estado = "Pendiente")
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";

        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        var productosPendientes = _context.ProductosPendientes
            .Where(pp => pp.EmpresaId == empresaId && pp.Estado == estado)
            .OrderBy(pp => pp.FechaCompra)
            .ToList();

        ViewBag.EstadoFiltro = estado;
        ViewBag.Estados = new[] { "Pendiente", "Procesado", "Cancelado" };

        return View(productosPendientes);
    }

    // 🔍 DETALLES
    public IActionResult Detalles(int id)
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";

        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        var productoPendiente = _context.ProductosPendientes.Find(id);
        if (productoPendiente == null)
        {
            return NotFound();
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;
        if (productoPendiente.EmpresaId != empresaId)
        {
            return Unauthorized();
        }

        return View(productoPendiente);
    }

    // ✏️ ACTUALIZAR ESTADO
    [HttpPost]
    public IActionResult ActualizarEstado(int id, string nuevoEstado)
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";

        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        var productoPendiente = _context.ProductosPendientes.Find(id);
        if (productoPendiente == null)
        {
            return NotFound();
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;
        if (productoPendiente.EmpresaId != empresaId)
        {
            return Unauthorized();
        }

        // Validar que el nuevo estado sea válido
        var estadosValidos = new[] { "Pendiente", "Procesado", "Cancelado" };
        if (!estadosValidos.Contains(nuevoEstado))
        {
            return BadRequest("Estado no válido");
        }

        productoPendiente.Estado = nuevoEstado;
        _context.SaveChanges();

        return RedirectToAction("Index", new { estado = nuevoEstado });
    }

    // 📊 RESUMEN
    public IActionResult Resumen()
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";

        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        var resumen = new
        {
            TotalPendiente = _context.ProductosPendientes
                .Where(pp => pp.EmpresaId == empresaId && pp.Estado == "Pendiente")
                .Sum(pp => pp.Cantidad - pp.CantidadProcesada),
            
            TotalProcesado = _context.ProductosPendientes
                .Where(pp => pp.EmpresaId == empresaId && pp.Estado == "Procesado")
                .Sum(pp => pp.Cantidad),
            
            TotalCancelado = _context.ProductosPendientes
                .Where(pp => pp.EmpresaId == empresaId && pp.Estado == "Cancelado")
                .Sum(pp => pp.Cantidad),
            
            RecuentoPendiente = _context.ProductosPendientes
                .Count(pp => pp.EmpresaId == empresaId && pp.Estado == "Pendiente"),
            
            RecuentoProcesado = _context.ProductosPendientes
                .Count(pp => pp.EmpresaId == empresaId && pp.Estado == "Procesado"),
            
            RecuentoCancelado = _context.ProductosPendientes
                .Count(pp => pp.EmpresaId == empresaId && pp.Estado == "Cancelado")
        };

        return View(resumen);
    }
}
