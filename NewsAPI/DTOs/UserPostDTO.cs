using System;
using System.Collections.Generic;

namespace NewsAPI.DTOs;

public partial class UserPostDTO
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }

    public int Status { get; set; }

    public DateTime PublishedDate { get; set; }
}
