using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NewsAPI.Controllers
{
    [Route("api/auth"), ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("google-signin")]
        public IActionResult GoogleSignIn()
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return BadRequest("Error authenticating with Google.");

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var userId = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // TODO: Create or find the user in your database

            // Now issue a JWT for your API
            var token = GenerateJwtToken(email, userId);

            return Ok(new { token });
        }

        private string GenerateJwtToken(string email, string userId)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

            var jwtSetting = _configuration.GetSection("JwtSettings");
            var secretKeyBytes = Encoding.UTF8.GetBytes(jwtSetting["SecretKey"]);
            var securityKey = new SymmetricSecurityKey(secretKeyBytes);
            var credetials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                 issuer: jwtSetting["Issuer"],
                 audience: jwtSetting["Audience"],
                 claims: claims,
                 expires: DateTime.UtcNow.AddMinutes(60),
                 signingCredentials: credetials
                 );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
