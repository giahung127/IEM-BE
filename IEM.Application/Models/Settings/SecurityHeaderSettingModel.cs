namespace IEM.Application.Models.Settings
{
    public class SecurityHeaderSettingModel
    {
        public string ReferrerPolicy { get; set; }
        public string XContentTypeOptions { get; set; }
        public string XFrameOptions { get; set; }
        public string XPermittedCrossDomainPolicies { get; set; }
        public string XXssProtection { get; set; }
        public string FeaturePolicy { get; set; }
        public string ContentSecurityPolicy { get; set; }
        public string CrossOriginResourcePolicy { get; set; }
        public string StrictTransportSecurity { get; set; }
    }
}
