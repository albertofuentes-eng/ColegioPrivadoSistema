using Microsoft.AspNetCore.Mvc;

namespace ColegioPrivado.Controllers
{
    public class SecretariaController : Controller
    {
        public IActionResult Panel()
        {
            return View();
        }
    }
}