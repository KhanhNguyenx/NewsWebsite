using Microsoft.AspNetCore.Mvc;

namespace NewsWebsite.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
