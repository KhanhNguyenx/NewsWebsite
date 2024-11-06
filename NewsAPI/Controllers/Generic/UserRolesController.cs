using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.DTOs;
using NewsAPI.Models;
using NewsAPI.Services;
using System.Linq.Expressions;

namespace NewsAPI.Controllers
{
    //[EnableRateLimiting("Fixed")]
    [Route("[controller]/[action]"), ApiController]
    public class RoleUsersController : ControllerBase
    {

        private readonly IGenericServive<RoleUser> _genericServive;
        private readonly IMapper _mapper;

        public RoleUsersController(IGenericServive<RoleUser> genericServive, IMapper mapper)
        {
            _genericServive = genericServive;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<RoleUserDTO>> Get(int id)
        {
            var entity = await _genericServive.GetAsync(id);
            if (entity != null)
            {
                var dto = new RoleUserDTO();
                _mapper.Map(entity, dto);
                return Ok(dto);
            }
            else
                return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<RoleUser>> GetFull(int id)
        {
            return await _genericServive.GetAsync(id);
        }
        [HttpPost]
        public async Task<ActionResult<RoleUserDTO>> Create(RoleUserDTO model)
        {
            Expression<Func<RoleUser, int>> filter = (x => x.Id);
            // Get Max Id in table of Database --> set for model + 1
            model.Id = await _genericServive.MaxIdAsync(filter) + 1;

            //Mapp data model --> newModel
            var newModel = new RoleUser();
            //newModel. = DateTime.Now;
            _mapper.Map(model, newModel);
            if (await _genericServive.CreateAsync(newModel) != null)
                return Ok(model);
            else
                return NoContent();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleUserDTO>>> GetList()
        {
            var entityList = await _genericServive.GetListAsync();
            if (entityList != null)
            {
                var dtoList = new List<RoleUserDTO>();
                _mapper.Map(entityList, dtoList);
                return Ok(dtoList);
            }
            else
                return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleUserDTO>>> Search(string txtSearch)
        {
            Expression<Func<RoleUser, bool>> filter = a =>
                a.Status != -1 &&
                (
                    (a.Account != null && a.Account.Username.Contains(txtSearch)) || // Tìm kiếm theo Username
                    (a.Role != null && a.Role.Name.Contains(txtSearch))        // Tìm kiếm theo Role Name
                );

            var entityList = await _genericServive.SearchAsync(filter);

            if (entityList != null && entityList.Any())
            {
                var dtoList = _mapper.Map<List<RoleUserDTO>>(entityList);
                return Ok(dtoList);
            }

            return NoContent();
        }


        //Delete Forever
        [HttpDelete("{id}")]
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
        //public async Task<ActionResult<RoleUserDTO>> Create(RoleUserDTO model)
        //{
        //    Expression<Func<RoleUser, int>> filter = (x => x.Id);
        //    // Get Max Id in table of Database --> set for model + 1
        //    model.Id = await _genericServive.MaxIdAsync(filter) + 1;

        //    //Mapp data model --> newModel
        //    var newModel = new RoleUser();
        //    //newModel. = DateTime.Now;
        //    _mapper.Map(model, newModel);

        //    if (await _genericServive.CreateAsync(newModel) != null)
        //        return Ok(model);
        //    else
        //        return NoContent();
        //}

        [HttpPut]
        public async Task<ActionResult<RoleUserDTO>> Update(RoleUserDTO model)
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
        [HttpGet]
        public async Task<IActionResult> GetPagedProducts(int pageNumber, int pageSize)
        {
            var (records, totalRecords) = await _genericServive.GetPagedListAsync(pageNumber, pageSize);
            var result = new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Records = _mapper.Map<IEnumerable<RoleUserDTO>>(records)
            };
            return Ok(result);
        }
    }
}
