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
                return RedirectToAction("Index");
            }

            PostDTO post = null;
            List<CategoryDTO> categoryList = new();
            List<ImageDTO> imageList = new();

            try
            {
                // Use Posts/Search API to find post by slug
                using (var searchResponse = await _client.GetAsync($"Posts/Search?txtSearch={slug}"))
                {
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
                }

                // Fetch the list of categories
                using (var categoryResponse = await _client.GetAsync("Categories/GetList"))
                {
                    if (categoryResponse.IsSuccessStatusCode)
                    {
                        var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                        categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(categoryData);
                    }
                }

                // Fetch the list of images
                using (var imageResponse = await _client.GetAsync("Images/GetList"))
                {
                    if (imageResponse.IsSuccessStatusCode)
                    {
                        var imageData = await imageResponse.Content.ReadAsStringAsync();
                        imageList = JsonConvert.DeserializeObject<List<ImageDTO>>(imageData);
                    }
                }
            }
            catch (Exception)
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

        [HttpGet("Category/{id}")]
        public async Task<IActionResult> PostsByCategory(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Category ID.";
                return RedirectToAction("Index");
            }
            List<PostDTO> postList = new();
            List<ImageDTO> imageList = new();
            List<CategoryDTO> categoryList = new();

            try
            {
                // Use Posts/GetList API to retrieve all posts and filter by categoryId
                using (var postResponse = await _client.GetAsync("Posts/GetList"))
                {
                    if (postResponse.IsSuccessStatusCode)
                    {
                        var postData = await postResponse.Content.ReadAsStringAsync();
                        var allPosts = JsonConvert.DeserializeObject<List<PostDTO>>(postData);

                        // Filter posts by categoryId
                        postList = allPosts.Where(post => post.CategoryId == id && post.Status == 1).ToList();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to retrieve posts for the specified category.";
                        return RedirectToAction("Index");
                    }
                }

                // Fetch the list of all images and filter based on postList
                using (var imageResponse = await _client.GetAsync("Images/GetList"))
                {
                    if (imageResponse.IsSuccessStatusCode)
                    {
                        var imageData = await imageResponse.Content.ReadAsStringAsync();
                        var allImages = JsonConvert.DeserializeObject<List<ImageDTO>>(imageData);

                        // Filter images based on PostId in postList
                        imageList = allImages.Where(image => postList.Any(post => post.Id == image.PostId) && image.Status == 1).ToList();
                    }
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while retrieving data.";
                return RedirectToAction("Index");
            }

            // Fetch the list of categories
            using (var categoryResponse = await _client.GetAsync("Categories/GetList"))
            {
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                    categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(categoryData);
                }
            }
            if (!postList.Any())
            {
                TempData["ErrorMessage"] = "No posts found for the specified category.";
            }

            // Pass the data to the view
            ViewBag.PostList = postList;
            ViewBag.ImageList = imageList;
            ViewBag.CategoryList = categoryList;
            return View();
        }
    }
}
