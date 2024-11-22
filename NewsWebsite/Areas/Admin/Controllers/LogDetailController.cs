using Microsoft.AspNetCore.Mvc;
using NewsAPI.Models;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace NewsWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LogDetailController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44358");
        private readonly HttpClient _client;

        public LogDetailController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        public async Task<IActionResult> Index()
        {
            List<Log> logList = new List<Log>();
            using (var response = await _client.GetAsync($"{_client.BaseAddress}Logs/GetFull"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    logList = JsonConvert.DeserializeObject<List<Log>>(data);
                }
            }
            return View(logList);
        }
    }
}
