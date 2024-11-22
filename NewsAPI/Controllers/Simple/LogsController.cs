using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.DTOs;
using NewsAPI.Models;
using NewsAPI.Services;

namespace NewsAPI.Controllers
{
    [Route("[controller]/[action]"), ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _Service;
        public LogsController(ILogService service)
        {
            _Service = service;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Log>>> GetFull()
        {
            var getfull = await _Service.GetListAsync();
            return getfull;
        }
    }
}
