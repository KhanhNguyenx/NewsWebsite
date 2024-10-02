using System;
using System.Collections.Generic;

namespace NewsAPI.Models;

public partial class Category
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public int ParentCategoryId { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
