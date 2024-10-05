using System;
using System.Collections.Generic;

namespace NewsAPI.Models;

public partial class CategoryDTO
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public int ParentCategoryId { get; set; }

    public int Status { get; set; }

}
