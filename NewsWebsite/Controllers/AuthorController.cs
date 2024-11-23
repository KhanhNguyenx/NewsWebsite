using Azure.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NewsWebsite.DTO;
using NewsWebsite.Helpers;
using NewsWebsite.Services;
using System.Security.Claims;
using System.Xml.Linq;
using NewsAPI.DTOs;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Win32;

namespace NewsWebsite.Controllers
{


    public class AuthorController : MyBaseController<AuthorController>
    {
        private readonly IConsumeApi _callApi;
        private readonly IConfiguration _configuration;
        public AuthorController(IConsumeApi consumeApi, IConfiguration configuration)
        {
            _callApi = consumeApi;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("GetList");

            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public async Task<IActionResult> GetList()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                //ResponseData res = new ResponseData();
                var res = await _callApi.GetAsync(@"Users/GetList", _accessToken);
                if (res.success && res.data != null)
                {
                    var resList = res.data!.ToObject<List<AccountDTO>>();
                    return View(resList);
                }
            }
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            //if (User.Identity != null && User.Identity.IsAuthenticated)
            //    return RedirectToAction("GetList");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Logins(LoginM user)
        {
            //user
            if (this.HttpContext.Request.Cookies.Count > 0)
            {
                this.HttpContext.Response.Cookies.Delete("Authentication");
                //foreach (var cookie in this.HttpContext.Request.Cookies.Keys)
                //    this.HttpContext.Response.Cookies.Delete(cookie);
            }
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            this.HttpContext.Session.Clear();

            ResponseData res = new ResponseData();
            if (ModelState.IsValid)
            {
                try
                {
                    Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
                    {
                        {"username", user.UserName},
                        {"passwordHash", user.Password },
                    };



                    res = await _callApi.PostAsync("Authorize/Login", dictPars);
                    if (res.success && res.data != null)
                    {
                        //////////////////////
                        AccountDTO account = res.data!.ToObject<AccountDTO>();
                        string token = res.message;
                        var getClaim = await _callApi.GetAsync(@"Authorize/GetClaims", token);
                        var getRoles = getClaim.data[4].value;
                        //Lưu accessToken vào biến sesion
                        HttpContext.Session.SetString(mySetting.AccessToken, token);

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, account.Email),                    // Email của người dùng
                            new Claim(ClaimTypes.Name, account.Username),                  // Tên người dùng
                            new Claim(ClaimTypes.Sid, account.Id.ToString()),              // ID của người dùng
                            new Claim("AccountId", account.Id.ToString()),                 // ID của tài khoản
                            new Claim("AccessToken", token),                               // Token truy cập
                            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),    // Identifier của người dùng
                            new Claim(ClaimTypes.Role, getRoles.ToString())
                        };

                        // Save Cookie
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
                        {
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
                            IsPersistent = true,
                            AllowRefresh = true
                        });
                        //return RedirectToAction("GetList");
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex)
                {
                    res.message = $"Exception: Xẩy ra lỗi khi đọc dữ liệu {ex.Message}";
                    return Redirect($"/error/404");
                }

            }
            return View(); // Redirect($"/error/404");
        }

        //[AllowAnonymous]
        //public IActionResult Register()
        //{
        //    return View();
        //}


        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> Registers(RegisterM registerModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(registerModel); // Trả về form nếu model không hợp lệ
        //    }

        //    ResponseData res = new ResponseData();
        //    try
        //    {
        //        // Chuẩn bị dữ liệu cho API đăng ký
        //        Dictionary<string, dynamic> dictParss = new Dictionary<string, dynamic>
        //{
        //    {"username", registerModel.UserName },
        //    {"passwordHash", registerModel.Password },
        //    {"email", registerModel.Email }
        //};

        //        // Gửi yêu cầu tới API "Authorize/Register"
        //        res = await _callApi.PostAsync("Authorize/Register", dictParss);

        //        if (res.success)
        //        {
        //            // Nếu đăng ký thành công, chuyển hướng tới trang login
        //            TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
        //            //return RedirectToAction("Login");
        //        }
        //        else
        //        {
        //            // Nếu API trả về lỗi
        //            ModelState.AddModelError("", res.message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", $"Đã xảy ra lỗi: {ex.Message}");
        //    }

        //    return View(registerModel);
        //}

        [AllowAnonymous]
        public async Task<ActionResult> Logout()
        {
            if (this.HttpContext.Request.Cookies.Count > 0)
            {
                this.HttpContext.Response.Cookies.Delete("Authentication");
                //foreach (var cookie in this.HttpContext.Request.Cookies.Keys)
                //    this.HttpContext.Response.Cookies.Delete(cookie);
            }
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            this.HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

       
    }
}