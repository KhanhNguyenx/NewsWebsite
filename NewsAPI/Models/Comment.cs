using System;
using System.Collections.Generic;

namespace NewsAPI.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public int? UserId { get; set; }

    public string CommentText { get; set; } = null!;

    public DateTime CommentDate { get; set; }

    public bool Status { get; set; }

    public virtual Post Post { get; set; } = null!;
}
