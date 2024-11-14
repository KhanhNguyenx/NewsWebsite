

namespace NewsWebsite.DTO
{
    public class RoleDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Notes { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public int Status { get; set; }

        public virtual ICollection<RoleUserDTO> RoleUsers { get; set; } = new List<RoleUserDTO>();
    }
}
