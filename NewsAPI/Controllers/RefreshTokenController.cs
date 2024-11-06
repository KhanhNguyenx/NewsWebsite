using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Services;
using NewsAPI.Services.SimpleService;

namespace NewsAPI.Controllers
{
    [Route("[controller]/[action]"), ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private IRefreshTokenService _refreshTokenService;
        private IUserService _userService;
        public RefreshTokenController(IRefreshTokenService refreshTokenService, IUserService userService)
        {
            _refreshTokenService = refreshTokenService;
            _userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(model.RefreshToken);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            var account = refreshToken.User;

            // Generate new access token
            var newAccessToken = await _userService.GenerateAccessTokenAsync(account);

            // Optionally, revoke the old refresh token and generate a new one
            await _refreshTokenService.RevokeRefreshTokenAsync(model.RefreshToken);
            var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(account);

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken.Token
            });
        }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }
}
