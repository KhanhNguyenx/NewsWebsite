using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NewsAPI.Controllers
{
    [Authorize, Route("api/protected"), ApiController]
    public class ProtectedController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetSecretData()
        {
            return Ok("This is a protected endpoint. Only accessible with a valid JWT.");
        }
    }
}
