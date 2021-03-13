using Microsoft.AspNetCore.Mvc;

namespace Teleport.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}