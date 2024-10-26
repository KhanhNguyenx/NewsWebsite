using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Models;
using NewsAPI.DTOs;
using NewsAPI.Services;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NewsAPI.Data;
using Microsoft.AspNetCore.Authorization;
using EncrypDecryp;
using System.Security.Cryptography;

namespace NewsAPI.Controllers.Generic
{
    [Route("[controller]/[action]"), ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IGenericServive<User> _genericServive;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly NewsWebDbContext _dbcontext;

        public UsersController(IGenericServive<User> genericServive, IMapper mapper, IConfiguration configuration, NewsWebDbContext newsWebDbContext)
        {
            _genericServive = genericServive;
            _mapper = mapper;
            _configuration = configuration;
            _dbcontext = newsWebDbContext;
        }
        private string GenerateAccessToken(User user)
        {
            Decryption decryption = new Decryption();
            var guid = Guid.NewGuid().ToString();
            var auThoClaims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Username", user.Username.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, guid),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };
            var jwtSetting = _configuration.GetSection("JwtSettings");
            var DecrypKey = jwtSetting["SecretKey"];
            var secretKeyBytes = Encoding.UTF8.GetBytes(/*decryption.Decrypt(*/DecrypKey)/*)*/;
            var securityKey = new SymmetricSecurityKey(secretKeyBytes);
            var credetials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                issuer: jwtSetting["Issuer"],
                audience: jwtSetting["Audience"],
                claims: auThoClaims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credetials
                );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }
        private string GenerateRefreshToken(User user)
        {
            var guid = Guid.NewGuid().ToString();
            var auThoClaims = new List<Claim>
    {
        new Claim("Id", user.Id.ToString()),
        new Claim("Username", user.Username.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, guid)
    };

            var jwtSetting = _configuration.GetSection("JwtSettings");
            var DecrypKey = jwtSetting["SecretKey"];
            var secretKeyBytes = Encoding.UTF8.GetBytes(/*decryption.Decrypt(*/DecrypKey)/*)*/;
            var securityKey = new SymmetricSecurityKey(secretKeyBytes);
            var credetials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var refreshToken = new JwtSecurityToken(
                issuer: jwtSetting["Issuer"],
                audience: jwtSetting["Audience"],
                claims: auThoClaims,
                expires: DateTime.UtcNow.AddDays(7), // Refresh Token sẽ có hiệu lực trong 7 ngày
                signingCredentials: credetials
            );

            var refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshToken);
            return refreshTokenString;
        }


        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            Decryption decryption = new Decryption();
            var decrypstring =/* decryption.Decrypt(*/_configuration["JwtSettings:SecretKey"]/*)*/;
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(decrypstring)),
                ClockSkew = TimeSpan.FromHours(1)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var user = _dbcontext.Users.SingleOrDefault(p => p.Username == loginModel.Username && loginModel.PasswordHash == p.PasswordHash);
            if (user == null)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid user/password"
                });
            }

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken(user);

            // Lưu RefreshToken vào cơ sở dữ liệu
            user.AccessToken = accessToken;
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh token có hiệu lực trong 7 ngày
            _dbcontext.SaveChanges();

            return Ok(new ApiResponse
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Message = "Login Success",
                Data = user,
            });
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] TokenModel tokenModel)
        {
            var principal = GetPrincipalFromExpiredToken(tokenModel.AccessToken);

            // Lấy claim 'Id' từ token để truy xuất người dùng
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "Id");
            if (userIdClaim == null)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var userId = userIdClaim.Value;
            var user = _dbcontext.Users.FirstOrDefault(u => u.Id == int.Parse(userId));

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid refresh token"
                });
            }


            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken(user);

            // Cập nhật RefreshToken mới
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _dbcontext.SaveChanges();

            return Ok(new ApiResponse
            {
                Success = true,
                Data = user
            });
        }



        [HttpGet("admin")]
        [Authorize(Policy = "RequireAdminRole")] // Chỉ cho phép admin
        public IActionResult AdminOnlyMethod()
        {
            var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(new { Message = "This is an admin-only endpoint.", Claims = userClaims });
        }

        [HttpGet("user")]
        [Authorize(Policy = "RequireUserRole")] // Cho phép cả user và admin
        public IActionResult UserOrAdminMethod()
        {
            return Ok(new { Message = "This is accessible by both users and admins." });
        }


        [HttpGet]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            var entity = await _genericServive.GetAsync(id);
            if (entity != null)
            {
                var dto = new UserDTO();
                _mapper.Map(entity, dto);
                return Ok(dto);
            }
            else
                return NoContent();
        }
        [HttpGet]
        public async Task<ActionResult<User>> GetFull(int id)
        {
            return await _genericServive.GetAsync(id);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserDTO>> Create([FromBody]UserDTO model)
        {
            if (ModelState.IsValid)
            {
                Expression<Func<User, int>> filter = (x => x.Id);
                // Get Max Id in table of Database --> set for model + 1
                model.Id = await _genericServive.MaxIdAsync(filter) + 1;

                //Mapp data model --> newModel
                var newModel = new User();
                //newModel. = DateTime.Now;
                _mapper.Map(model, newModel);

                if (await _genericServive.CreateAsync(newModel) != null)
                    return Ok(model);
            }
            return NoContent();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetList()
        {
            var entityList = await _genericServive.GetListAsync();
            if (entityList != null)
            {
                var dtoList = new List<UserDTO>();
                _mapper.Map(entityList, dtoList);
                return Ok(dtoList);
            }
            else
                return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> Search(string txtSearch)
        {
            Expression<Func<User, bool>> filter;
            filter = a => a.Status != -1 && (a.Role!.Contains(txtSearch) || a.FullName!.Contains(txtSearch));
            var entityList = await _genericServive.SearchAsync(filter);
            if (entityList != null)
            {
                var dtoList = new List<UserDTO>();
                _mapper.Map(entityList, dtoList);
                return Ok(dtoList);
            }
            else
                return NoContent();
        }

        //Delete Forever
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var entity = _genericServive.GetAsync(id);
            if (entity != null)
            {
                var result = _genericServive.Delete(entity.Result);
                if (result > 0)
                    return Ok("Record is deleted");
            }
            return NotFound();
        }

        //Delete Fake and Update status to 0
        [HttpDelete]
        public async Task<ActionResult> UpDelete(int id)
        {
            var entity = await _genericServive.GetAsync(id); // Dùng await để lấy kết quả bất đồng bộ
            if (entity != null)
            {
                var result = await _genericServive.DeleteAsync(entity); // Chờ đợi quá trình xóa logic (thay đổi trạng thái)
                if (result > 0)
                    return Ok("Record status has been updated to 'deleted'"); // Thay đổi message cho rõ ràng hơn
            }
            return NotFound();
        }

        //Another Way to Create...
        //[HttpPost]
        //public async Task<ActionResult<UsersDTO>> Create(UsersDTO model)
        //{
        //    Expression<Func<Users, int>> filter = (x => x.Id);
        //    // Get Max Id in table of Database --> set for model + 1
        //    model.Id = await _genericServive.MaxIdAsync(filter) + 1;

        //    //Mapp data model --> newModel
        //    var newModel = new Users();
        //    //newModel. = DateTime.Now;
        //    _mapper.Map(model, newModel);

        //    if (await _genericServive.CreateAsync(newModel) != null)
        //        return Ok(model);
        //    else
        //        return NoContent();
        //}

        [HttpPut]
        public async Task<ActionResult<UserDTO>> Update(UserDTO model)
        {
            var entity = await _genericServive.GetAsync(model.Id);
            if (entity != null)
            {
                _mapper.Map(model, entity);
                if (await _genericServive.UpdateAsync(entity) != null)
                    return Ok(model);
            }
            return NotFound();
        }
    }
}
