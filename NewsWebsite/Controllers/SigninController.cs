using Microsoft.AspNetCore.Mvc;

namespace NewsWebsite.Controllers
{
    public class SigninController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
