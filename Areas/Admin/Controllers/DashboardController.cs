﻿using Microsoft.AspNetCore.Mvc;

namespace NewsWebsite.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
