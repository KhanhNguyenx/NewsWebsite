using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace NewsAPI.Middleware
{
    public class AntiXSSMiddleware
    {
        private readonly RequestDelegate _next;
        public AntiXSSMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.QueryString.HasValue)
            {
                foreach( var query in context.Request.Query)
                {
                    if (ContainsXss(query.Value.ToString()))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Bad Request: Potential XSS attack detected");
                        return;
                    }
                }
                await _next(context);
            }
        }
        private bool ContainsXss(string input)
        {
            if(string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            var xssPatten = @"<.*> | javascript: | onerror | onload | eval | <script.*?> | </script>";
            return Regex.IsMatch(input, xssPatten, RegexOptions.IgnoreCase);
        }
    }
}
