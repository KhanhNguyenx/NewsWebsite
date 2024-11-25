using Microsoft.AspNetCore.Mvc;
using NewsAPI.DTOs;
using NewsWebsite.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace NewsWebsite.Controllers
{
    public class HomeController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44358");
        private readonly HttpClient _client;
        public HomeController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    List<PostDTO> postList = new List<PostDTO>();
        //    List<CategoryDTO> categoryList = new List<CategoryDTO>();
        //    List<ImageDTO> imageList = new List<ImageDTO>();

        //    // Fetch the list of posts from the API
        //    using (var postResponse = await _client.GetAsync("Posts/GetList"))
        //    {
        //        if (postResponse.IsSuccessStatusCode)
        //        {
        //            var postData = await postResponse.Content.ReadAsStringAsync();
        //            postList = JsonConvert.DeserializeObject<List<PostDTO>>(postData);
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Failed to retrieve posts list!";
        //        }
        //    }

        //    // Fetch the list of categories from the API
        //    using (var categoryResponse = await _client.GetAsync("Categories/GetList"))
        //    {
        //        if (categoryResponse.IsSuccessStatusCode)
        //        {
        //            var categoryData = await categoryResponse.Content.ReadAsStringAsync();
        //            categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(categoryData);
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Failed to retrieve categories list!";
        //        }
        //    }

        //    // Fetch the list of images from the API
        //    using (var imageResponse = await _client.GetAsync("Images/GetList"))
        //    {
        //        if (imageResponse.IsSuccessStatusCode)
        //        {
        //            var imageData = await imageResponse.Content.ReadAsStringAsync();
        //            imageList = JsonConvert.DeserializeObject<List<ImageDTO>>(imageData);
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Failed to retrieve images list!";
        //        }
        //    }

        //    // Đóng gói dữ liệu để truyền xuống View
        //    ViewBag.PostList = postList;
        //    ViewBag.CategoryList = categoryList;
        //    ViewBag.ImageList = imageList;

        //    return View();
        //}
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5)
        {
            List<PostDTO> pagedPostList = new List<PostDTO>();
            List<CategoryDTO> categoryList = new List<CategoryDTO>();
            List<ImageDTO> imageList = new List<ImageDTO>();
            List<PostDTO> allPosts = new List<PostDTO>();
            int totalRecords = 0;

            // Fetch the paged list of posts from the API
            using (var postResponse = await _client.GetAsync($"Posts/GetPagedProducts?pageNumber={pageNumber}&pageSize={pageSize}"))
            {
                if (postResponse.IsSuccessStatusCode)
                {
                    var postData = await postResponse.Content.ReadAsStringAsync();
                    var pagedResult = JsonConvert.DeserializeObject<PagedResult<PostDTO>>(postData);

                    // Gán giá trị từ pagedResult
                    pagedPostList = pagedResult?.Records ?? new List<PostDTO>();
                    totalRecords = pagedResult?.TotalRecords ?? 0;

                    // Lọc các bài viết có Status = 1
                    pagedPostList = pagedPostList.Where(p => p.Status == 1).ToList();
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve paged posts list!";
                }
            }

            // Fetch the list of categories from the API
            using (var categoryResponse = await _client.GetAsync("Categories/GetList"))
            {
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                    categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(categoryData);
                    ViewBag.CategoryList = categoryList;
                }
            }

            // Fetch the list of images from the API
            using (var imageResponse = await _client.GetAsync("Images/GetList"))
            {
                if (imageResponse.IsSuccessStatusCode)
                {
                    var imageData = await imageResponse.Content.ReadAsStringAsync();
                    imageList = JsonConvert.DeserializeObject<List<ImageDTO>>(imageData);
                    ViewBag.ImageList = imageList;
                }
            }

            // Fetch the list of all posts from the API
            using (var postResponse = await _client.GetAsync("Posts/GetList"))
            {
                if (postResponse.IsSuccessStatusCode)
                {
                    var postData = await postResponse.Content.ReadAsStringAsync();
                    allPosts = JsonConvert.DeserializeObject<List<PostDTO>>(postData);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve posts list!";
                }
            }

            // Lọc ra 3 bài viết có số lượt xem cao nhất
            var popularPosts = allPosts.Where(p => p.Status == 1).OrderByDescending(p => p.Views).Take(3).ToList();
            ViewBag.PopularPosts = popularPosts;

            // Lọc ra 3 bài viết có số lượt thích cao nhất
            var topLikedPosts = allPosts.Where(p => p.Status == 1).OrderByDescending(p => p.LikeNumber).Take(3).ToList();
            ViewBag.TopLikedPosts = topLikedPosts;

            // Calculate total pages for pagination
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;

            // Pass the paged list of posts to the view
            return View(pagedPostList);
        }



        public IActionResult Privacy()
        {
            return View();
        }
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
