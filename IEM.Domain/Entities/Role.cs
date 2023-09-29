namespace IEM.Domain.Entities
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public string? RoleName { get; set; }
        public int RoleCode { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
