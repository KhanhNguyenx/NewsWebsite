using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace NewsWebsite
{
    public class CheckCookieExpirationMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckCookieExpirationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Kiểm tra nếu user đã xác thực (authenticated)
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                // Kiểm tra thời gian hết hạn cookie
                var expireClaim = context.User.Claims.FirstOrDefault(c => c.Type == "exp");
                if (expireClaim != null)
                {
                    var expireTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expireClaim.Value));
                    if (expireTime < DateTimeOffset.UtcNow)
                    {
                        // Cookie đã hết hạn, yêu cầu đăng nhập lại
                        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        context.Response.Redirect("/Account/Login");
                        return;
                    }
                }
            }
            // Gọi middleware tiếp theo
            await _next(context);
        }
    }
}
