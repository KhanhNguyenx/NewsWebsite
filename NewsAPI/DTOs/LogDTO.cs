using System;
using System.Collections.Generic;
namespace NewsAPI.DTOs;

public partial class LogDTO
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime ActionDate { get; set; }

    public string? Details { get; set; }

    public int? EntityId { get; set; }
    public int Status { get; set; }

    public string? EntityType { get; set; }
}
