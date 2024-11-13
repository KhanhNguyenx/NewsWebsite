using Microsoft.AspNetCore.Mvc;
using NewsAPI.DTOs;
using Newtonsoft.Json;
using System.Net.Http;

namespace NewsWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44358");
        private readonly HttpClient _client;

        public PostController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Upsert()
        {
            List<CategoryDTO> categoryList = new List<CategoryDTO>();
            using (var response = await _client.GetAsync("Categories/GetList"))
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
            ViewBag.CategoryList = categoryList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpsertAsync(PostDTO model, List<IFormFile> Images)
        {
            // Step 1: Create the Post using /post/create API
            var response = await _client.PostAsJsonAsync("Posts/Create", model);
            if (!response.IsSuccessStatusCode)
            {
                return View(model); // Handle error
            }

            var createdPost = await response.Content.ReadFromJsonAsync<PostDTO>();

            // Step 2: Save each image and create records with /image/create API
            var uploadPath = Path.Combine("DATA", "Images");
            Directory.CreateDirectory(uploadPath);

            foreach (var image in Images)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Create an ImageDTO and save to /image/create API
                var imageDTO = new ImageDTO
                {
                    PostId = createdPost.Id,
                    ImageUrl = Path.Combine("DATA", "Images", fileName),
                    Caption = "", // Optionally set a caption
                    Status = 1 // Example status value
                };

                await _client.PostAsJsonAsync("Images/Create", imageDTO);
            }

            return RedirectToAction("Index"); // Or another view upon success
        }
    }
}
