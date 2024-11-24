using NewsAPI.Models;

namespace NewsAPI.DTOs
{
    public class UserRoleOutDTO
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public bool IsAuthor { get; set; }

        public int Status { get; set; }

        public string? Notes { get; set; }

        public List<string> Roles { get; set; }
    }
}
