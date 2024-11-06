using System;
using System.Collections.Generic;

namespace NewsAPI.Models;

public partial class GoogleUser
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime CreateAt { get; set; }
}
