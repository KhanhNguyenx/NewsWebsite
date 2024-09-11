using Microsoft.AspNetCore.Mvc;

namespace NewsWebsite.Controllers
{
    public class DetailsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
