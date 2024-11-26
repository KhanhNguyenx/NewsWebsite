using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NewsAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace NewsWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles ="Admin")]
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
                // If the model state is not valid, re-populate the category list for the view
                await PopulateCategoryList();
                return View(model);
            }

            try
            {
                string data = JsonConvert.SerializeObject(model);
                using (StringContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage response;

                    if (model.Id == 0)
                    {
                        // New category, call the Create API
                        response = await _client.PostAsync($"{_client.BaseAddress}Categories/Create", content);
                    }
                    else
                    {
                        // Existing category, call the Update API
                        response = await _client.PutAsync($"{_client.BaseAddress}Categories/Update", content);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["successMessage"] = model.Id == 0 ? "Category Created!" : "Category Updated!";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            // Re-populate the category list for the view if there's an error
            await PopulateCategoryList();
            return View(model);
        }

        // Helper method to populate CategoryList
        private async Task PopulateCategoryList()
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

            ViewBag.CategoryList = categoryList;
        }
    }
}