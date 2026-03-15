using Microsoft.AspNetCore.Mvc;

namespace ColegioPrivado.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
