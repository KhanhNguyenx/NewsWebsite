

namespace NewsWebsite.DTO
{
    public class RoleUserDTO
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public int AccountId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public int Status { get; set; }

        public virtual AccountDTO Account { get; set; } = null!;

        public virtual RoleDTO Role { get; set; } = null!;
    }
}
