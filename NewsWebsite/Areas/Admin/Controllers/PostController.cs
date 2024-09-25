using Microsoft.AspNetCore.Mvc;

namespace NewsWebsite.Areas.Admin.Controllers
{
    public class PostController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert ()
        {
            return View();
        }
    }
}
