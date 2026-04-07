using Microsoft.AspNetCore.Mvc;
using ColegioPrivado.Data;
using System.Linq;

namespace ColegioPrivado.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            // 🔒 Verificar sesión
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // 🔒 Verificar rol
            if (HttpContext.Session.GetString("Rol") != "Administrador")
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            // 📊 ESTADÍSTICAS
            ViewBag.TotalUsuarios = _context.Usuarios.Count();

            ViewBag.UsuariosActivos = _context.Usuarios
                .Count(u => u.Estado == true);

            ViewBag.UsuariosBloqueados = _context.Usuarios
                .Count(u => u.Estado == false);

            ViewBag.SolicitudesPendientes = _context.SolicitudesAccesos
                .Count(s => s.Estado == "Pendiente");

            return View();
        }
    }
}
