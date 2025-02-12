﻿using System;
using System.Collections.Generic;

namespace NewsAPI.Models;

public partial class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;

    public DateTime Created { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;

    public DateTime? Revoked { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
