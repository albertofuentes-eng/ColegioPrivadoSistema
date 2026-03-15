using Microsoft.AspNetCore.Mvc;

namespace ColegioPrivado.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}