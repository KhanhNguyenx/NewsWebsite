using Microsoft.AspNetCore.Mvc;
using NewsAPI.Models;
using Newtonsoft.Json;
using NewsAPI.Data;
using NewsAPI.DTOs;
using NewsWebsite.Models;
using System.Text;

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
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Categories/GetList").Result;
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
            List<CategoryDTO> categoryList = new List<CategoryDTO>();

            // Gọi API lấy danh sách danh mục
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Categories/GetList").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(data);
            }

            // Kiểm tra xem categoryList có dữ liệu hay không
            if (categoryList == null || categoryList.Count == 0)
            {
                // Thêm thông báo lỗi nếu không có dữ liệu
                TempData["ErrorMessage"] = "No categories found!";
                return View(); // Trả về view mà không có danh sách
            }

            // Gán dữ liệu vào ViewBag
            ViewBag.CategoryList = categoryList;
            var model = new CategoryDTO();
            return View(model);

        }
        [HttpPost]
        public IActionResult Upsert(CategoryDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, encoding: System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "Categories/Create", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Category Created!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }
        [HttpGet]
        public IActionResult Detail()
        {
            //CategoryDTO record = new CategoryDTO();
            //HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Categories/Get" + id).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    string data = response.Content.ReadAsStringAsync().Result;
            //    record = JsonConvert.DeserializeObject<CategoryDTO>(data);
            //}
            //return View(record);
            return View();
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                CategoryDTO record = new CategoryDTO();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Categories/Get" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    record = JsonConvert.DeserializeObject<CategoryDTO>(data);
                }
                return View(record);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult deletedConfirm(int id)
        {
            try
            {
                HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "Categories/Delete" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Category Deleted!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

    }
}