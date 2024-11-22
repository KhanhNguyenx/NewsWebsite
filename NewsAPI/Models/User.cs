using System;
using System.Collections.Generic;

namespace NewsAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public bool IsAuthor { get; set; }

    public int Status { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<RoleUser> RoleUsers { get; set; } = new List<RoleUser>();

    public virtual ICollection<UserPost> UserPosts { get; set; } = new List<UserPost>();

    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
