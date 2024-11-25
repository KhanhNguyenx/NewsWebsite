using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.DTOs;
using NewsWebsite.Helpers;
using NewsWebsite.Services;
using System.Security.Claims;

namespace NewsWebsite.Controllers
{
    public class UsersController : MyBaseController<UsersController>
    {
        private readonly IConsumeApi _callApi;
        private readonly IConfiguration _configuration;
        public UsersController(IConsumeApi consumeApi, IConfiguration configuration)
        {
            _callApi = consumeApi;
            _configuration = configuration;
        }
        [Authorize]
        public async Task<IActionResult> Details(string id)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Lấy ID từ claim nếu không có giá trị truyền vào
                id ??= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Login", "Author");
                }

                //var token = HttpContext.Session.GetString(mySetting.AccessToken);
                //if (string.IsNullOrEmpty(token))
                //{
                //    return RedirectToAction("Login", "Author");
                //}

                // Gọi API lấy thông tin user
                var res = await _callApi.GetAsync($"Users/Get/{id}", _accessToken);
                if (res.success && res.data != null)
                {
                    var user = res.data.ToObject<UserDTO>();
                    return View(user);
                }
            }
            // Chuyển hướng tới trang login nếu có lỗi
            //return RedirectToAction("Login", "Author");
            return View();
        }

    }
}
