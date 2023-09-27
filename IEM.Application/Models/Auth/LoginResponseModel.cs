namespace IEM.Application.Models.Auth
{
    public class LoginResponseModel
    {
        public string? AccessToken { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
    }
}
