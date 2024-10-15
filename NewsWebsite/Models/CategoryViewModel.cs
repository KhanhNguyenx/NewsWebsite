using NewsAPI.DTOs;

namespace NewsWebsite.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }

        public int ParentCategoryId { get; set; }

        public int Status { get; set; }
    }
}
