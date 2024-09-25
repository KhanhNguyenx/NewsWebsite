using System;
using System.Collections.Generic;

namespace NewsAPI.Models;

public partial class Log
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime ActionDate { get; set; }

    public string? Details { get; set; }

    public int? EntityId { get; set; }

    public string? EntityType { get; set; }
}
