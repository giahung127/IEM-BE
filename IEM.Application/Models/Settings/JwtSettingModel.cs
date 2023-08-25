namespace IEM.Application.Models.Settings
{
    public class JwtSettingModel
    {
        public string AccessTokenSecret { get; set; }
        public string RefreshTokenSecret { get; set; }
        public string AccessTokenName { get; set; }
        public string RefreshTokenName { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string SecurityAlgorithm { get; set; }
        public double AccessTokenExpirationDays { get; set; }
        public double RefreshTokenExpirationDays { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool RequireExpirationTime { get; set; }
        public bool RequireSignedTokens { get; set; }
        public bool RequireAudience { get; set; }
    }
}
