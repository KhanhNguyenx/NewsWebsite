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
using NewsAPI.Services.SimpleService;
using NewsAPI.Models.APIHelperModels;

namespace NewsAPI.Controllers.Generic
{
    [Route("[controller]/[action]"), ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IGenericServive<User> _genericServive;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IGenericServive<User> genericServive, IMapper mapper, IUserService user)
        {
            _genericServive = genericServive;
            _mapper = mapper;
            _userService = user;
        }
        [HttpGet]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            var entity = await _userService.GetAsync(id);
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
        [Authorize("RequireAdminRole")]
        public async Task<ActionResult<User>> GetFull(int id)
        {
            return await _userService.GetAsync(id);
        }
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<UserDTO>> Create([FromBody]UserDTO model)
        {
            if (ModelState.IsValid)
            {
                // Get Max Id in table of Database --> set for model + 1
                model.Id = await _userService.MaxIdAsync(model.Id) + 1;

                //Mapp data model --> newModel
                var newModel = new User();
                //newModel. = DateTime.Now;
                _mapper.Map(model, newModel);

                if (await _userService.CreateAsync(newModel) != null)
                    return Ok(model);
            }
            return NoContent();
        }
        [HttpGet]
        //[Authorize("RequireAdminRole")]
        public async Task<ActionResult<DataResponse>> GetList()
        {
            var entityList = await _userService.GetListAsync();
            if (entityList != null)
            {

                var response = new DataResponse
                {
                    Success = true,
                    Message = "Data fetched successfully",
                    Data = entityList
                };
                return Ok(response);
            }
            else
            {
                return NoContent();
            }
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> Search(string txtSearch)
        {
            Expression<Func<User, bool>> filter;
            filter = a => a.Status != -1 && (a.FullName!.Contains(txtSearch));
            var entityList = await _userService.SearchAsync(filter);
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
            var result = _userService.Delete(id);
            if (result == 1)
            {
                return Ok(new { success = true, message = "Record is deleted." });
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
            var entity = await _userService.GetAsync(model.Id);
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
