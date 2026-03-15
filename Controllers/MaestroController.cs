using Microsoft.AspNetCore.Mvc;

namespace ColegioPrivado.Controllers
{
    public class MaestroController : Controller
    {
        public IActionResult Panel()
        {
            return View();
        }
    }
}