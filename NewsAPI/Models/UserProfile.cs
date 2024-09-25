using System;
using System.Collections.Generic;

namespace NewsAPI.Models;

public partial class UserProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public string? SocialLinks { get; set; }

    public virtual User User { get; set; } = null!;
}
