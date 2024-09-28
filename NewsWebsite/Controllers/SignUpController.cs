using Microsoft.AspNetCore.Mvc;

namespace NewsWebsite.Controllers
{
    public class SignUpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
