using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NewsAPI.Data;
using NewsAPI.Services.SimpleService;
using NewsAPI.Services;
using AutoMapper;
using NewsAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using NewsAPI.Models;
using NewsAPI.Controllers.Generic;
using NewsAPI.Models.APIHelperModels;
using Microsoft.AspNetCore.Authorization;

namespace NewsAPI.Controllers
{
    [Route("[controller]/[action]"), ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IGenericServive<RoleUser> _genericServive;
        private readonly IRefreshTokenService _refreshToken;
        private readonly NewsWebDbContext _dbContext;
        private readonly IMapper _mapper;

        public AuthorizeController(IConfiguration configuration, IUserService userService, IRefreshTokenService refreshToken, IGenericServive<RoleUser> genericServive ,NewsWebDbContext dbContext, IMapper mapper)
        {
            _configuration = configuration;
            _userService = userService;
            _genericServive = genericServive;
            _refreshToken = refreshToken;
            _dbContext = dbContext;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Login([FromBody] LoginModel model)
        {
            //{
            //    "userName": "JohnDoe_1",
            //    "password": "Password@123"
            //}
            if (!ModelState.IsValid)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid login data"
                });
            }

            // Sử dụng repository để lấy tài khoản
            var account = await _userService.GetAccountByUsernameAsync(model.Username);
            
            if (account == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid username or password"
                });
            }

            // Kiểm tra mật khẩu qua repository
            if (!await _userService.VerifyPasswordAsync(account, model.PasswordHash))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid username or password"
                });
            }
            var user = new UserDTO
            {
                Id = account.Id,
                Username = account.Username,
                PasswordHash = account.PasswordHash,
                Email = account.Email,
                FullName = account.FullName,
                IsAuthor = account.IsAuthor,
                Status = account.Status,
                Notes = account.Notes,
            };
            // Tạo token qua repository
            var token = await _userService.GenerateAccessTokenAsync(account);

            var refreshToken = await _refreshToken.GenerateRefreshTokenAsync(account);

            // Trả về kết quả đăng nhập thành công
            return Ok(new
            {
                success = true,
                message = token,
                data = user,
                refreshToken = refreshToken.Token
            });
        }
        [HttpPost]
        public async Task<ActionResult> RegisterAsync(
                                [FromBody] RegisterModel model,
                                [FromServices] IServiceProvider serviceProvider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy instance của UsersController và RoleUsersController từ serviceProvider
            var usersController = serviceProvider.GetRequiredService<UsersController>();
            var roleUsersController = serviceProvider.GetRequiredService<RoleUsersController>();

            // Kiểm tra người dùng đã tồn tại chưa
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Email);
            if (existingUser != null)
            {
                return Conflict(new { success = false, message = "Người dùng đã tồn tại." });
            }

            // Mã hóa mật khẩu
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
            //var newAccount = new User
            //{
            //    Username = model.Username,
            //    PasswordHash = hashedPassword,
            //    Email = model.Email,
            //    FullName =model.Username,
            //};

            // Tạo UserDTO từ newAccount
            var newAccountDTO = new UserDTO
            {
                Username = model.Username,
                PasswordHash = model.PasswordHash,
                Email = model.Email,
                FullName =model.Username,
                Status = model.Status
            };
            var user = new UserDTO
            {
                Username = model.Username,
                PasswordHash = model.PasswordHash
            };

            try
            {
                // Gọi phương thức Create trong UsersController để thêm User vào cơ sở dữ liệu
                var userCreateResult = await usersController.Create(newAccountDTO);

                if (userCreateResult is NoContentResult)
                {
                    return BadRequest(new { success = false, message = "Không thể tạo người dùng mới." });
                }

                // Lấy vai trò "User" từ bảng Roles
                var userRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                if (userRole == null)
                {
                    return BadRequest(new { success = false, message = "Vai trò mặc định không tồn tại." });
                }

                // Gán vai trò "User" cho người dùng mới thông qua RoleUsersController
                var roleUserDto = new RoleUserDTO
                {
                    RoleId = userRole.Id,
                    AccountId = newAccountDTO.Id, // Lấy Id từ newAccountDTO đã được thêm vào DB
                    CreatedAt = DateTime.UtcNow,
                    Status = 1
                };

                // Gọi phương thức Create trong RoleUsersController để thêm RoleUser
                var roleCreateResult = await roleUsersController.Create(roleUserDto);

                if (roleCreateResult is NoContentResult)
                {
                    return BadRequest(new { success = false, message = "Không thể gán vai trò cho người dùng." });
                }
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest(new { success = false, message = "Lỗi khi lưu tài khoản: " + dbEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi không xác định: " + ex.Message });
            }

            return Ok(new { success = true, data = user });
        }
        [Authorize]
        [HttpGet]
        public DataResponse GetClaims()
        {
            if (User.Identity!.IsAuthenticated)
            {
                // Lấy tất cả các claims của người dùng đã đăng nhập
                var claims = User.Claims;
                // Duyệt qua danh sách claims và in ra các giá trị của chúng
                var claimsList = claims.Select(c => new
                {
                    Type = c.Type,
                    Value = c.Value
                }).ToList();
                return new DataResponse(true, claimsList);
            }
            return new DataResponse();
        }



        [HttpGet("google-signin")]
        public IActionResult GoogleSignIn()
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return BadRequest("Error authenticating with Google.");

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var userId = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Create or find the user in the database here

            // Create ClaimsPrincipal for the user
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, email),
        new Claim(ClaimTypes.NameIdentifier, userId)
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // Keep user signed in
            };

            // Sign in using Cookies
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // Optionally generate a JWT token if needed
            var jwtToken = GenerateJwtToken(email, userId);

            // Return the JWT token or redirect after signing in
            return Ok(new { Token = jwtToken }); // Adjust as needed
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
