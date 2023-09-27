using System.ComponentModel.DataAnnotations.Schema;

namespace IEM.Domain.Entities
{
    public class User
    {
        public long Id { get; set; }
        public required string Mobile { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public ICollection<UserConnection> UserConnections { get; set; } = new List<UserConnection>();
    }
}
