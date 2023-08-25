using IEM.Application.Models.Constants;
using IEM.Application.Models.Settings;
using IEM.Application.Utils;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace IEM.WebAPI.Middlewares
{
    public sealed class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;
        public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext context, IOptions<SecurityHeaderSettingModel> options, IOptions<AntiforgeryOptions> antiforgeryOptions)
        {
            if (context.Request.Path.StartsWithSegments("/api") || context.Request.Path.StartsWithSegments("/swagger"))
            {
                _logger.LogInformation("SecurityHeadersMiddleware -> Invoke -> start");
                var countryName = string.Empty;
                var countryCode = string.Empty;

                var ipAddress = HttpContextUtils.GetIpAdress(context);
                _logger.LogInformation($"SecurityHeadersMiddleware -> Invoke -> IPAddress: {ipAddress}");

                var ipAddressLoging = HttpContextUtils.GetIpAdressLogging(context);
                _logger.LogInformation($"SecurityHeadersMiddleware -> Invoke -> IPAddress Loggin: {ipAddressLoging}");

                var originDomainName = context.Request.Path.StartsWithSegments("/api") ? HttpContextUtils.GetOrigin(context) : context.Request.Host.Host;
                _logger.LogInformation($"SecurityHeadersMiddleware -> Invoke -> DamianName: {originDomainName}");

            }

            context.Response.OnStarting(() =>
            {
                var settings = options.Value;
                if (settings != null)
                {
                    if (!string.IsNullOrEmpty(settings.StrictTransportSecurity))
                    {
                        context.Response.Headers.Remove(HeaderConstants.STRICT_TRANSPORT_SECURITY);
                        context.Response.Headers.Add(HeaderConstants.STRICT_TRANSPORT_SECURITY, new StringValues(settings.StrictTransportSecurity));
                    }

                    if (!string.IsNullOrEmpty(settings.CrossOriginResourcePolicy))
                    {
                        context.Response.Headers.Remove(HeaderConstants.CROSS_ORIGIN_RESOURCE_POLICY);
                        context.Response.Headers.Add(HeaderConstants.CROSS_ORIGIN_RESOURCE_POLICY, new StringValues(settings.CrossOriginResourcePolicy));
                    }

                    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
                    if (!string.IsNullOrEmpty(settings.ReferrerPolicy))
                    {
                        context.Response.Headers.Remove(HeaderConstants.REFERRER_POLICY);
                        context.Response.Headers.Add(HeaderConstants.REFERRER_POLICY, new StringValues(settings.ReferrerPolicy));
                    }

                    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
                    if (!string.IsNullOrEmpty(settings.XContentTypeOptions))
                    {
                        context.Response.Headers.Remove(HeaderConstants.X_CONTENTTYPE_OPTIONS);
                        context.Response.Headers.Add(HeaderConstants.X_CONTENTTYPE_OPTIONS, new StringValues(settings.XContentTypeOptions));
                    }

                    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
                    if (!string.IsNullOrEmpty(settings.XFrameOptions)
                        && (antiforgeryOptions.Value == null || !antiforgeryOptions.Value.SuppressXFrameOptionsHeader))
                    {
                        context.Response.Headers.Remove(HeaderConstants.X_FRAME_OPTIONS);
                        context.Response.Headers.Add(HeaderConstants.X_FRAME_OPTIONS, new StringValues(settings.XFrameOptions));
                    }

                    // https://security.stackexchange.com/questions/166024/does-the-x-permitted-cross-domain-policies-header-have-any-benefit-for-my-websit
                    if (!string.IsNullOrEmpty(settings.XPermittedCrossDomainPolicies))
                    {
                        context.Response.Headers.Remove(HeaderConstants.X_PERMITTED_CROSS_DOMAIN_POLICIES);
                        context.Response.Headers.Add(HeaderConstants.X_PERMITTED_CROSS_DOMAIN_POLICIES, new StringValues(settings.XPermittedCrossDomainPolicies));
                    }

                    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-XSS-Protection
                    if (!string.IsNullOrEmpty(settings.XXssProtection))
                    {
                        context.Response.Headers.Remove(HeaderConstants.X_XSS_PROTECTION);
                        context.Response.Headers.Add(HeaderConstants.X_XSS_PROTECTION, new StringValues(settings.XXssProtection));
                    }
                    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Feature-Policy
                    // https://github.com/w3c/webappsec-feature-policy/blob/master/features.md
                    // https://developers.google.com/web/updates/2018/06/feature-policy
                    if (!string.IsNullOrEmpty(settings.FeaturePolicy))
                    {
                        context.Response.Headers.Remove(HeaderConstants.FEATURE_POLICY);
                        context.Response.Headers.Add(HeaderConstants.FEATURE_POLICY, new StringValues(settings.FeaturePolicy));
                    }

                    // https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP
                    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
                    if (!string.IsNullOrEmpty(settings.ContentSecurityPolicy))
                    {
                        context.Response.Headers.Remove(HeaderConstants.CONTENT_SECURITY_POLICY);
                        context.Response.Headers.Add(HeaderConstants.CONTENT_SECURITY_POLICY, new StringValues(settings.ContentSecurityPolicy));
                    }
                }

                context.Response.Headers.Remove(HeaderConstants.SERVER);
                context.Response.Headers.Remove(HeaderConstants.X_POWERD_BY);
                context.Response.Headers.Remove(HeaderConstants.X_ASPNET_VERSION);
                return Task.CompletedTask;
            });

            return _next(context);
        }
    }
}
