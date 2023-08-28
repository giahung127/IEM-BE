using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IEM.Domain.Entities
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        public required string Mobile { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
