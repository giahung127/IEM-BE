namespace IEM.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string? RoleName { get; set; }
        public int RoleCode { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
