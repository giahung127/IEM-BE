namespace IEM.Application.Models.Auth
{
    public class TokenValidationModel
    {
        public int UserId { get; set; }
        public bool IsValid { get; set; }
    }
}
