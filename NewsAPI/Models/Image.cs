using System;
using System.Collections.Generic;

namespace NewsAPI.Models;

public partial class Image
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? Caption { get; set; }

    public int Status { get; set; }

    public virtual Post Post { get; set; } = null!;
}
