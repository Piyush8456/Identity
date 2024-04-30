using Microsoft.AspNetCore.Mvc;

namespace Custom_Identity.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Unauthorized()
        {
            return View();
        }
    }
}
