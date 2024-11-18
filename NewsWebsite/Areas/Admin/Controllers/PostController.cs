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
        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    List<PostDTO> postList = new List<PostDTO>();
        //    List<CategoryDTO> categoryList = new List<CategoryDTO>();

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
        //            ViewBag.CategoryList = categoryList; // Store in ViewBag for use in the view
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Failed to retrieve categories list!";
        //        }
        //    }

        //    // Pass the list of posts to the view
        //    return View(postList);
        //}
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
        public async Task<IActionResult> Upsert(PostDTO model, List<IFormFile> Images)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Step 1: Tạo bài viết thông qua API /post/create
            using (var response = await _client.PostAsJsonAsync("Posts/Create", model))
            {
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to create post.");
                    return View(model); // Xử lý lỗi nếu không thành công
                }

                var createdPost = await response.Content.ReadFromJsonAsync<PostDTO>();

                // Step 2: Lưu từng ảnh vào thư mục và tạo bản ghi bằng API /image/create
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DATA", "Images");
                Directory.CreateDirectory(uploadPath); // Tạo thư mục nếu chưa tồn tại

                foreach (var image in Images)
                {
                    if (image == null || image.Length == 0)
                    {
                        continue;
                    }

                    // Tạo tên file duy nhất bằng Guid và lấy phần mở rộng từ file gốc
                    var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Sử dụng `using` để đảm bảo FileStream được giải phóng sau khi sử dụng
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Tạo một ImageDTO và lưu bằng API /image/create
                    var imageDTO = new ImageDTO
                    {
                        PostId = createdPost.Id,
                        ImageUrl = Path.Combine("DATA", "Images", fileName), // Đường dẫn tương đối tới ảnh
                        Caption = "", // Bạn có thể thêm chú thích nếu cần
                        Status = 1 // Ví dụ trạng thái giá trị
                    };

                    // Gửi yêu cầu tạo ảnh đến API
                    using (var imageResponse = await _client.PostAsJsonAsync("Images/Create", imageDTO))
                    {
                        if (!imageResponse.IsSuccessStatusCode)
                        {
                            // Xử lý lỗi nếu không thể tạo ảnh
                            ModelState.AddModelError("", "Error saving image.");
                            return View(model);
                        }
                    }
                }
            }
            return RedirectToAction("Index"); // Hoặc điều hướng tới một view khác khi thành công
        }



        //[HttpGet]
        //public async Task<IActionResult> Upsert(int id = 0)
        //{
        //    // Lấy danh sách danh mục cho dropdown
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

        //    // Nếu có id, lấy thông tin bài viết và danh sách hình ảnh
        //    if (id != 0)
        //    {
        //        var post = new PostDTO();
        //        var imageList = new List<ImageDTO>();

        //        // Lấy thông tin bài viết
        //        using (var postResponse = await _client.GetAsync($"Posts/Get?id={id}"))
        //        {
        //            if (postResponse.IsSuccessStatusCode)
        //            {
        //                var postData = await postResponse.Content.ReadAsStringAsync();
        //                post = JsonConvert.DeserializeObject<PostDTO>(postData);
        //            }
        //            else
        //            {
        //                TempData["ErrorMessage"] = "Failed to retrieve post!";
        //                return RedirectToAction("Index"); // Quay lại trang danh sách nếu lỗi
        //            }
        //        }

        //        // Lấy danh sách hình ảnh
        //        using (var imageResponse = await _client.GetAsync("Images/GetList"))
        //        {
        //            if (imageResponse.IsSuccessStatusCode)
        //            {
        //                var imageData = await imageResponse.Content.ReadAsStringAsync();
        //                var allImages = JsonConvert.DeserializeObject<List<ImageDTO>>(imageData);
        //                imageList = allImages.Where(img => img.PostId == id).ToList();
        //            }
        //            else
        //            {
        //                TempData["ErrorMessage"] = "Failed to retrieve images!";
        //            }
        //        }

        //        // Truyền danh sách hình ảnh vào ViewBag
        //        ViewBag.Images = imageList;

        //        return View(post); // Trả về view với dữ liệu bài viết và hình ảnh
        //    }

        //    return View(); // Trả về form trống nếu tạo bài viết mới
        //}

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
