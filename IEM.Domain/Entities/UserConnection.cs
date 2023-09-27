using System.ComponentModel.DataAnnotations.Schema;

namespace IEM.Domain.Entities
{
    public class UserConnection
    {
        public long Id { get; set; }
        public required string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTimeOffset AccessTokenExpiredDate { get; set; }
        public DateTimeOffset? RefreshTokenExpirationDate { get; set; }
        //public string DeviceKey { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
        //public int DeviceType { get; set; }

        // User foreginer key
        [ForeignKey("User")]
        public long UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
