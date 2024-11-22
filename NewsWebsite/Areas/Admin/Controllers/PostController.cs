using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.DTOs;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web;

namespace NewsWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class PostController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44358");
        private readonly HttpClient _client;

        public PostController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<PostDTO> postList = new List<PostDTO>();
            List<CategoryDTO> categoryList = new List<CategoryDTO>();
            List<ImageDTO> imageList = new List<ImageDTO>();

            // Fetch the list of posts from the API
            using (var postResponse = await _client.GetAsync("Posts/GetList"))
            {
                if (postResponse.IsSuccessStatusCode)
                {
                    var postData = await postResponse.Content.ReadAsStringAsync();
                    postList = JsonConvert.DeserializeObject<List<PostDTO>>(postData);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve posts list!";
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
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve categories list!";
                }
            }

            // Fetch the list of images from the API
            using (var imageResponse = await _client.GetAsync("Images/GetList"))
            {
                if (imageResponse.IsSuccessStatusCode)
                {
                    var imageData = await imageResponse.Content.ReadAsStringAsync();
                    imageList = JsonConvert.DeserializeObject<List<ImageDTO>>(imageData);
                    ViewBag.ImageList = imageList; // Store in ViewBag for use in the view
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve images list!";
                }
            }

            // Pass the list of posts to the view
            return View(postList);
        }


        // Controller - Upsert Methods
        //[HttpGet]
        //public async Task<IActionResult> Upsert()
        //{
        //    List<CategoryDTO> categoryList = new List<CategoryDTO>();
        //    using (var response = await _client.GetAsync("Categories/GetList"))
        //    {
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var data = await response.Content.ReadAsStringAsync();
        //            categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(data);
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Failed to retrieve categories list!";
        //        }
        //    }
        //    ViewBag.CategoryList = categoryList;
        //    return View();
        //}

        [HttpGet]
        public async Task<IActionResult> Upsert(int id = 0)
        {
            // Lấy danh sách danh mục cho dropdown
            ViewBag.CategoryList = await GetCategoryList();

            if (id != 0) // Nếu chỉnh sửa bài viết
            {
                var post = await GetPostById(id);
                if (post == null)
                {
                    TempData["ErrorMessage"] = "Failed to retrieve post!";
                    return RedirectToAction("Index");
                }

                // Giải mã nội dung bài viết để hiển thị đúng trên CKEditor
                //post.Contents = HttpUtility.HtmlDecode(post.Contents);
                var imageList = await GetImagesByPostId(id);
                ViewBag.Images = imageList;

                return View(post);
            }

            return View(); // Tạo bài viết mới
        }


        [HttpPost]
        public async Task<IActionResult> Upsert(PostDTO model, List<IFormFile> Images)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = await GetCategoryList();
                return View(model);
            }

            bool isCreate = model.Id == 0; // Xác định là tạo mới hay cập nhật

            // Mã hóa nội dung bài viết trước khi gửi đến API
            //model.Contents = HttpUtility.HtmlEncode(model.Contents);

            HttpResponseMessage response;
            if (isCreate)
            {
                response = await _client.PostAsJsonAsync("Posts/Create", model);
            }
            else
            {
                response = await _client.PutAsJsonAsync("Posts/Update", model);
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = isCreate ? "Failed to create post!" : "Failed to update post!";
                ViewBag.CategoryList = await GetCategoryList();
                return View(model);
            }

            var post = isCreate ? await response.Content.ReadFromJsonAsync<PostDTO>() : model;

            if (Images != null && Images.Count > 0)
            {
                await SaveImages(post.Id, Images);
            }

            TempData["SuccessMessage"] = isCreate ? "Post created successfully!" : "Post updated successfully!";
            return RedirectToAction("Index");
        }


        // Phương thức phụ để lấy danh sách danh mục
        private async Task<List<CategoryDTO>> GetCategoryList()
        {
            var categoryList = new List<CategoryDTO>();
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
            return categoryList;
        }

        // Phương thức phụ để lấy bài viết theo ID
        private async Task<PostDTO?> GetPostById(int id)
        {
            using (var response = await _client.GetAsync($"Posts/Get?id={id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var postData = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<PostDTO>(postData);
                }
            }
            return null;
        }

        // Phương thức phụ để lấy danh sách hình ảnh của bài viết
        private async Task<List<ImageDTO>> GetImagesByPostId(int postId)
        {
            var imageList = new List<ImageDTO>();
            using (var response = await _client.GetAsync("Images/GetList"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var imageData = await response.Content.ReadAsStringAsync();
                    var allImages = JsonConvert.DeserializeObject<List<ImageDTO>>(imageData);
                    imageList = allImages.Where(img => img.PostId == postId).ToList();
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to retrieve images!";
                }
            }
            return imageList;
        }

        // Phương thức phụ để lưu ảnh
        private async Task SaveImages(int postId, List<IFormFile> Images)
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DATA", "Images");
            Directory.CreateDirectory(uploadPath); // Tạo thư mục nếu chưa tồn tại

            foreach (var image in Images)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var imageDTO = new ImageDTO
                {
                    PostId = postId,
                    ImageUrl = Path.Combine("DATA", "Images", fileName),
                    Caption = string.Empty,
                    Status = 1
                };

                using (var response = await _client.PostAsJsonAsync("Images/Create", imageDTO))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Failed to save some images!";
                        break;
                    }
                }
            }
        }





        //[HttpPost]
        //public async Task<IActionResult> Upsert(PostDTO model, List<IFormFile> Images)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        // Re-populate danh sách danh mục nếu có lỗi
        //        await PopulateCategoryList();
        //        return View(model);
        //    }

        //    if (model.Id == 0) // Tạo bài viết mới
        //    {
        //        using (var response = await _client.PostAsJsonAsync("Posts/Create", model))
        //        {
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                TempData["ErrorMessage"] = "Failed to create post!";
        //                await PopulateCategoryList();
        //                return View(model);
        //            }

        //            var createdPost = await response.Content.ReadFromJsonAsync<PostDTO>();

        //            // Lưu ảnh sau khi tạo bài viết
        //            await SaveImages(createdPost.Id, Images);
        //        }
        //    }
        //    else // Cập nhật bài viết
        //    {
        //        using (var response = await _client.PutAsJsonAsync("Posts/Update", model))
        //        {
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                TempData["ErrorMessage"] = "Failed to update post!";
        //                await PopulateCategoryList();
        //                return View(model);
        //            }

        //            // Lưu ảnh sau khi cập nhật bài viết
        //            await SaveImages(model.Id, Images);
        //        }
        //    }

        //    TempData["SuccessMessage"] = model.Id == 0 ? "Post created successfully!" : "Post updated successfully!";
        //    return RedirectToAction("Index"); // Hoặc điều hướng tới view khác khi thành công
        //}

        //private async Task SaveImages(int postId, List<IFormFile> Images)
        //{
        //    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DATA", "Images");
        //    Directory.CreateDirectory(uploadPath); // Tạo thư mục nếu chưa tồn tại

        //    foreach (var image in Images)
        //    {
        //        // Tạo tên file duy nhất
        //        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
        //        var filePath = Path.Combine(uploadPath, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await image.CopyToAsync(stream);
        //        }

        //        // Gửi yêu cầu tạo ảnh đến API
        //        var imageDTO = new ImageDTO
        //        {
        //            PostId = postId,
        //            ImageUrl = Path.Combine("DATA", "Images", fileName),
        //            Caption = string.Empty,
        //            Status = 1
        //        };

        //        using (var imageResponse = await _client.PostAsJsonAsync("Images/Create", imageDTO))
        //        {
        //            if (!imageResponse.IsSuccessStatusCode)
        //            {
        //                TempData["ErrorMessage"] = "Failed to save some images!";
        //            }
        //        }
        //    }
        //}

        //private async Task PopulateCategoryList()
        //{
        //    List<CategoryDTO> categoryList = new List<CategoryDTO>();
        //    using (var response = await _client.GetAsync("Categories/GetList"))
        //    {
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var data = await response.Content.ReadAsStringAsync();
        //            categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(data);
        //        }
        //    }
        //    ViewBag.CategoryList = categoryList;
        //}
    }
}
