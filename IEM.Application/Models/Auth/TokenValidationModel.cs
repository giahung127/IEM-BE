namespace IEM.Application.Models.Auth
{
    public class TokenValidationModel
    {
        public Guid UserId { get; set; }
        public bool IsValid { get; set; }
    }
}
