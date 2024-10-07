using System;
using System.Collections.Generic;

namespace NewsAPI.DTOs;

public partial class ImageDTO
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? Caption { get; set; }

    public int Status { get; set; }

}
