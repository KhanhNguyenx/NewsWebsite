using System;
using System.Collections.Generic;
namespace NewsAPI.DTOs;

public partial class UserProfileDTO
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }
    public int Status { get; set; }

    public string? SocialLinks { get; set; }

}
