using System.ComponentModel.DataAnnotations;

namespace IEM.Application.Models.Auth
{
    public class LoginRequestModel
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
