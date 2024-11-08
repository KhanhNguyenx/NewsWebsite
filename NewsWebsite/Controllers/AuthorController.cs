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

namespace NewsWebsite.Controllers
{
    public class AuthorController : MyBaseController<AuthorController>
    {
        private readonly IConsumeApi _callApi;
        private readonly IConfiguration _configuration;
        public AuthorController(IConsumeApi callApi, IConfiguration configuration)
        {
            _callApi = callApi;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("GetList");

            return RedirectToAction("Login");
        }
        [Authorize]
        public async Task<IActionResult> GetList()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                //ResponseData res = new ResponseData();
                var res = await _callApi.GetAsync(@"Categories/GetList", _accessToken);
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
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("GetList");

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Logins(LoginM user)
        {
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
                        {"userName", user.UserName},
                        {"password", user.Password },
                    };



                    res = await _callApi.PostAsync("Authorize/Login", dictPars);
                    if (res.success && res.data != null)
                    {
                        //////////////////////
                        AccountDTO account = res.data!.ToObject<AccountDTO>();
                        string token = res.message;
                        //Lưu accessToken vào biến sesion
                        HttpContext.Session.SetString(mySetting.AccessToken, token);

                        var claims = new List<Claim> {
                                            new Claim(ClaimTypes.Email, account.Email ),
                                            new Claim(ClaimTypes.Name, account.Username),
                                            new Claim(ClaimTypes.Sid, account.Id.ToString()),
                                            new Claim("AccountId", account.Id.ToString()),
                                            new Claim("AccessToken", token),
                                            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                                            //new Claim(ClaimTypes.Role,account.RoleId.ToString())
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
                        return RedirectToAction("Index", "Product");
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
            return RedirectToAction("Login");
        }
    }
}
