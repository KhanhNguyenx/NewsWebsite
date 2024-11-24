using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.DTOs;
using NewsAPI.Models.APIHelperModels;
using NewsWebsite.Controllers;
using NewsWebsite.DTO;
using NewsWebsite.Services;
using Newtonsoft.Json;

namespace NewsWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : MyBaseController<UserController>
    {
        private readonly IConsumeApi _callApi;
        private readonly IConfiguration _configuration;
        public UserController(IConsumeApi consumeApi, IConfiguration configuration)
        {
            _callApi = consumeApi;
            _configuration = configuration;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                //ResponseData res = new ResponseData();
                var res = await _callApi.GetAsync(@"Users/GetList", _accessToken);
                if (res.success && res.data != null)
                {
                    var resList = res.data!.ToObject<List<UserRoleOutDTO>>();
                    return View(resList);
                }
            }
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Details(string id)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var res = await _callApi.GetAsync($"Users/GetById/{id}", _accessToken);
                if (res.success && res.data != null)
                {
                    var resList = res.data!.ToObject<UserRoleOutDTO>();
                    return View(resList);
                }
            }
            return RedirectToAction("Login");
        }
    }
}
