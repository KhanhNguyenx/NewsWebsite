using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Models;
using NewsAPI.Services;
using System.Linq.Expressions;

namespace NewsAPI.Controllers
{
    [Route("[controller]/[action]"), ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IGenericServive<Category> _genericServive;
        private readonly IMapper _mapper;
        public CategoriesController(IGenericServive<Category> genericServive, IMapper mapper)
        {
            _genericServive = genericServive;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<CategoryDTO>> Get(int id) 
        {
            var entity = await _genericServive.GetASync(id);
            if (entity != null)
            {
                var dto = new CategoryDTO();
                _mapper.Map(entity, dto);
                return Ok(dto);
            }
            else
                return NoContent();
        }
        [HttpGet]
        public async Task<ActionResult<Category>> GetFull(int id)
        {
            return await _genericServive.GetASync(id);
        }
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> Create(CategoryDTO model)
        {
            Expression<Func<Category, int>> filter = (x => x.Id);
            // Get Max Id in table of Database --> set for model + 1
            model.Id = await _genericServive.MaxIdAsync(filter) + 1;

            //Mapp data model --> newModel
            var newModel = new Category();
            //newModel. = DateTime.Now;
            _mapper.Map(model, newModel);

            if (await _genericServive.CreateASync(newModel) != null)
                return Ok(model);
            else
                return NoContent();
        }
    }
}
