using Microsoft.AspNetCore.Mvc;

namespace ColegioPrivado.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Dashboard()
        {
            // Verificar si el usuario inició sesión
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Verificar que el rol sea Usuario Final
            if (HttpContext.Session.GetString("Rol") != "Usuario Final")
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            return View();
        }
    }
}