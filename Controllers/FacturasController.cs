using Microsoft.AspNetCore.Mvc;
using ColegioPrivado.Data;
using ColegioPrivado.Models;
using System.Linq;
using Rotativa.AspNetCore;

public class FacturasController : Controller
{
    private readonly ApplicationDbContext _context;

    public FacturasController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";
        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;
        var facturas = _context.Facturas
            .Where(f => f.EmpresaId == empresaId)
            .ToList();

        return View(facturas);
    }

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

        return View();
    }

    [HttpPost]
    public IActionResult Create(Factura factura)
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";
        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;
        var compra = _context.Compras
            .FirstOrDefault(c => c.CompraId == factura.CompraId && c.EmpresaId == empresaId);

        if (compra == null)
        {
            ModelState.AddModelError("", "Compra seleccionada no válida.");
            ViewBag.Compras = _context.Compras
                .Where(c => c.EmpresaId == empresaId)
                .ToList();
            return View(factura);
        }

        var facturaExistente = _context.Facturas
            .FirstOrDefault(f => f.CompraId == factura.CompraId && f.EmpresaId == empresaId);

        if (facturaExistente != null)
        {
            ModelState.AddModelError("", "Ya existe una factura para esta compra.");
            ViewBag.Compras = _context.Compras
                .Where(c => c.EmpresaId == empresaId)
                .ToList();
            return View(factura);
        }

        factura.EmpresaId = empresaId;
        factura.Fecha = DateTime.Now;
        factura.Total = compra.Total;
        factura.NumeroFactura = GenerarNumeroFactura(empresaId);

        _context.Facturas.Add(factura);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult ExportarPdf(int id)
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";
        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;
        var factura = _context.Facturas
            .FirstOrDefault(f => f.FacturaId == id && f.EmpresaId == empresaId);

        if (factura == null)
        {
            return NotFound();
        }

        return new ViewAsPdf("FacturaPdf", factura)
        {
            FileName = $"Factura_{factura.NumeroFactura}.pdf"
        };
    }

    public IActionResult VerFactura(int id)
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";
        if (!rol.Contains("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;
        var factura = _context.Facturas
            .FirstOrDefault(f => f.FacturaId == id && f.EmpresaId == empresaId);

        if (factura == null)
        {
            return NotFound();
        }

        return View("FacturaVista", factura);
    }

    private string GenerarNumeroFactura(int empresaId)
    {
        var consecutivo = _context.Facturas
            .Count(f => f.EmpresaId == empresaId) + 1;

        return $"FAC-{DateTime.Now:yyyyMMdd}-{consecutivo:D4}";
    }
}
