using Microsoft.AspNetCore.Mvc;
using ColegioPrivado.Data;
using ColegioPrivado.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class ProveedorController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProveedorController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        var proveedores = await _context.Proveedores
            .Where(p => p.EmpresaId == empresaId)
            .ToListAsync();

        return View(proveedores);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Proveedor proveedor)
    {
        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        if (!ModelState.IsValid)
            return View(proveedor);

        proveedor.EmpresaId = empresaId;

        _context.Proveedores.Add(proveedor);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleActivo(int id)
    {
        int empresaId = HttpContext.Session.GetInt32("EmpresaId") ?? 1;

        var proveedor = await _context.Proveedores
            .FirstOrDefaultAsync(p => p.ProveedorId == id && p.EmpresaId == empresaId);

        if (proveedor == null)
            return NotFound();

        proveedor.Activo = !proveedor.Activo;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}