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
            var guid = Guid.NewGuid().ToString();
            var auThoClaims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Username", user.Username.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role,"Role"),
                new Claim(JwtRegisteredClaimNames.Jti, guid),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };
            var jwtSetting = _configuration.GetSection("JwtSettings");
            var secretKeyBytes = Encoding.UTF8.GetBytes(jwtSetting["SecretKey"]);
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

        [HttpPost]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var user = _dbcontext.Users.SingleOrDefault(p=> p.Username == loginModel.Username && loginModel.PasswordHash == p.PasswordHash);
            if (user == null) 
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid user/password"
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                
                Message = GenerateAccessToken(user),
                Data = user,
            });
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
        
        public async Task<ActionResult<UserDTO>> Create(UserDTO model)
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
            else
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
