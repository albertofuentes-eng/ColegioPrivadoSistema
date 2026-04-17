using Microsoft.AspNetCore.Mvc;
using ColegioPrivado.Data;
using ColegioPrivado.Models;
using System.Linq;

public class EmpresasController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmpresasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔹 LISTAR
    public IActionResult Index()
    {
        var activas = _context.Empresas.Where(e => e.Activo).ToList();
        var inactivas = _context.Empresas.Where(e => !e.Activo).ToList();

        ViewBag.Activas = activas;
        ViewBag.Inactivas = inactivas;

        return View();
    }

    // 🔹 CREAR (GET)
    public IActionResult Create()
    {
        return View();
    }

    // 🔹 CREAR (POST)
    [HttpPost]
    public IActionResult Create(Empresa empresa)
    {
        if (!ModelState.IsValid)
            return View(empresa);

        empresa.Activo = true;

        _context.Empresas.Add(empresa);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // 🔹 CAMBIAR ESTADO
    public IActionResult Toggle(int id)
    {
        var empresa = _context.Empresas.Find(id);

        if (empresa != null)
        {
            empresa.Activo = !empresa.Activo;
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}