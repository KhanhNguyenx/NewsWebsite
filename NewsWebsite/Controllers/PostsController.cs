using Microsoft.AspNetCore.Mvc;
using NewsAPI.DTOs;
using Newtonsoft.Json;

namespace NewsWebsite.Controllers
{
    public class PostsController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44358");
        private readonly HttpClient _client;

        public PostsController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        [HttpGet("{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                TempData["ErrorMessage"] = "Slug is missing.";
                return RedirectToAction("Index"); // Redirect to the index page if slug is null
            }

            PostDTO post = null;
            List<CategoryDTO> categoryList = new();
            List<ImageDTO> imageList = new();

            try
            {
                // Use Posts/Search API to find post by slug
                var searchResponse = await _client.GetAsync($"Posts/Search?txtSearch={slug}");
                if (searchResponse.IsSuccessStatusCode)
                {
                    var searchData = await searchResponse.Content.ReadAsStringAsync();
                    var searchResults = JsonConvert.DeserializeObject<List<PostDTO>>(searchData);

                    // Expecting only one result with the matching slug
                    post = searchResults.FirstOrDefault();
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve post details.";
                    return RedirectToAction("Index");
                }

                // Fetch the list of categories
                var categoryResponse = await _client.GetAsync("Categories/GetList");
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                    categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(categoryData);
                }

                // Fetch the list of images
                var imageResponse = await _client.GetAsync("Images/GetList");
                if (imageResponse.IsSuccessStatusCode)
                {
                    var imageData = await imageResponse.Content.ReadAsStringAsync();
                    imageList = JsonConvert.DeserializeObject<List<ImageDTO>>(imageData);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while retrieving data.";
                return RedirectToAction("Index");
            }

            if (post == null)
            {
                TempData["ErrorMessage"] = "Post not found.";
                return RedirectToAction("Index");
            }

            // Pass the data to the view
            ViewBag.CategoryList = categoryList;
            ViewBag.ImageList = imageList;
            return View(post);
        }

    }
}
