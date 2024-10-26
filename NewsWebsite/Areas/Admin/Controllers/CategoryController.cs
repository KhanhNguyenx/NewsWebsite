using Microsoft.AspNetCore.Mvc;
using NewsAPI.Models;
using Newtonsoft.Json;
using NewsAPI.Data;
using NewsAPI.DTOs;
using NewsWebsite.Models;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        public async Task<IActionResult> Index()
        {
            List<CategoryDTO> categoryList = new List<CategoryDTO>();
            using (var response = await _client.GetAsync($"{_client.BaseAddress}Categories/GetList"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(data);
                }
            }
            return View(categoryList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CategoryDTO> categoryList = new List<CategoryDTO>();

            // Gọi API lấy danh sách danh mục
            using (var response = await _client.GetAsync($"{_client.BaseAddress}Categories/GetList"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(data);
                }
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
        public async Task<IActionResult> Create(CategoryDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Trả về view với model để hiển thị lỗi
            }

            try
            {
                string data = JsonConvert.SerializeObject(model);
                using (StringContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage response = await _client.PostAsync($"{_client.BaseAddress}Categories/Create", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["successMessage"] = "Category Created!";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(model); // Trả về view với model nếu có lỗi
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                CategoryDTO category = null; // Khởi tạo category là null
                using (var response = await _client.GetAsync($"{_client.BaseAddress}/Categories/Get/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        category = JsonConvert.DeserializeObject<CategoryDTO>(data);
                    }
                    else
                    {
                        TempData["errorMessage"] = "Category not found!";
                        return RedirectToAction("Index"); // Quay về trang Index nếu không tìm thấy
                    }
                }

                if (category == null)
                {
                    TempData["errorMessage"] = "Category not found!";
                    return RedirectToAction("Index");
                }

                return View(category);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Index"); // Quay về trang Index nếu có lỗi
            }
        }

        [HttpPost]
        public IActionResult Edit(CategoryDTO model)
        {
            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/Categories/Update", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Category Updated!";
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