using Microsoft.AspNetCore.Mvc;

namespace ColegioPrivado.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            // Verificar si el usuario inició sesión
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Verificar que el rol sea Administrador
            if (HttpContext.Session.GetString("Rol") != "Administrador")
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            return View();
        }
    }
}
