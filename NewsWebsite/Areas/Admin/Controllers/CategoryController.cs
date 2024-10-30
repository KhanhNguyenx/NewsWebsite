using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        public async Task<IActionResult> Upsert(int id = 0)
        {
            // Initialize an empty model for the view
            var model = new CategoryDTO();

            // Fetch the list of categories for the ParentCategory dropdown
            List<CategoryDTO> categoryList = new List<CategoryDTO>();
            using (var response = await _client.GetAsync($"{_client.BaseAddress}Categories/GetList"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(data);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve categories list!";
                }
            }

            // Assign category list to ViewBag for the dropdown in the view
            ViewBag.CategoryList = categoryList;

            // If id is not 0, fetch the specific category details for editing
            if (id != 0)
            {
                using (var response = await _client.GetAsync($"{_client.BaseAddress}Categories/Get?id={id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        model = JsonConvert.DeserializeObject<CategoryDTO>(data);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to retrieve category details!";
                        return View(model); // Return the view with an error message
                    }
                }
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Upsert(CategoryDTO model)
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

        //[HttpGet("{id}")]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    try
        //    {
        //        CategoryDTO category = null; // Khởi tạo category là null
        //        using (var response = await _client.GetAsync($"{_client.BaseAddress}Categories/Get?{id}"))
        //        {
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var data = await response.Content.ReadAsStringAsync();
        //                category = JsonConvert.DeserializeObject<CategoryDTO>(data);
        //            }
        //            else
        //            {
        //                TempData["errorMessage"] = "Category not found!";
        //                return RedirectToAction("Index"); // Quay về trang Index nếu không tìm thấy
        //            }
        //        }

        //        if (category == null)
        //        {
        //            TempData["errorMessage"] = "Category not found!";
        //            return RedirectToAction("Index");
        //        }

        //        return View(category);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["errorMessage"] = ex.Message;
        //        return RedirectToAction("Index"); // Quay về trang Index nếu có lỗi
        //    }
        //}

        //[HttpPost]
        //public IActionResult Edit(CategoryDTO model)
        //{
        //    try
        //    {
        //        string data = JsonConvert.SerializeObject(model);
        //        StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
        //        HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/Categories/Update", content).Result;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            TempData["successMessage"] = "Category Updated!";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["errorMessage"] = ex.Message;
        //        return View();
        //    }
        //    return View();
        //}
    }
}