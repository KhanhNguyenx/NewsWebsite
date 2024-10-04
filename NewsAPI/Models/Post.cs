using System;
using System.Collections.Generic;

namespace NewsAPI.Models;

public partial class Post
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Contents { get; set; } = null!;

    public int CategoryId { get; set; }

    public bool Status { get; set; }

    public int Views { get; set; }

    public bool IsHot { get; set; }

    public int LikeNumber { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<UserPost> UserPosts { get; set; } = new List<UserPost>();
}
