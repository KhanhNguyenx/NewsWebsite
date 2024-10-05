using System;
using System.Collections.Generic;

namespace NewsAPI.Models;

public partial class UserPostDTO
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }

    public int Status { get; set; }

    public DateTime PublishedDate { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
