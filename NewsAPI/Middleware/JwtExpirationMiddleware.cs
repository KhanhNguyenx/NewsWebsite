using System.IdentityModel.Tokens.Jwt;

namespace NewsAPI.Middleware
{
    public class JwtExpirationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtExpirationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Giải mã token để kiểm tra expiration
                var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
                if (jwtToken != null)
                {
                    var expireTime = jwtToken.ValidTo;

                    // Nếu token đã hết hạn
                    if (expireTime < DateTime.UtcNow)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token expired. Please login again.");
                        return;
                    }
                }
            }
            // Gọi middleware tiếp theo
            await _next(context);
        }
    }
}
