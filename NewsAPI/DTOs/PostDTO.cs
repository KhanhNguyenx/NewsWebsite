using System;
using System.Collections.Generic;

namespace NewsAPI.DTOs;

public partial class PostDTO
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Contents { get; set; } = null!;

    public int CategoryId { get; set; }

    public int Status { get; set; }

    public int Views { get; set; }

    public bool IsHot { get; set; }

    public int LikeNumber { get; set; }
}
