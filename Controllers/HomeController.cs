using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ColegioPrivado.Models;

namespace ColegioPrivado.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var usuario = HttpContext.Session.GetString("Usuario");
        var rol = HttpContext.Session.GetString("Rol");

        if (usuario != null)
        {
            if (rol == "Administrador")
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            if (rol == "Usuario Final")
            {
                return RedirectToAction("Dashboard", "Usuario");
            }
        }

        return View();
    }

    public IActionResult AccesoDenegado()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
