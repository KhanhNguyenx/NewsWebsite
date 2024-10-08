using Microsoft.AspNetCore.Mvc;
using NewsAPI.Models;
using Newtonsoft.Json;
using NewsAPI.Data;
using NewsAPI.DTOs;

namespace NewsWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44358");
        private readonly HttpClient _client;

        public CategoryController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<CategoryDTO> categoryList = new List<CategoryDTO>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Categories/GetList").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(data);
            }
            return View(categoryList);
        }
        [HttpGet]
        public IActionResult Upsert()
        {
            return View();
        }
    }
}