using System;
using System.Collections.Generic;

namespace NewsAPI.DTOs;

public partial class CommentDTO
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public int? UserId { get; set; }

    public string CommentText { get; set; } = null!;

    public DateTime CommentDate { get; set; }

    public int Status { get; set; }
}
