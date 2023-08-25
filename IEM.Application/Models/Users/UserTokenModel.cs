namespace IEM.Application.Models.Users
{
    public class UserTokenModel
    {
        public string Email { get; set; }
        public Guid UserId { get; set; }
        public int AdminAccess { get; set; }
        public string Audience { get; set; }
    }
}
