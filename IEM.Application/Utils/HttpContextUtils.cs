using IEM.Application.Models.Constants;
using IEM.Application.Models.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using System.Net.Sockets;

namespace IEM.Application.Utils
{
    public static class HttpContextUtils
    {
        public static string GetAuthority(this HttpContext httpContext)
        {
            var httpRequest = httpContext?.Request;
            if (httpRequest != null)
            {
                return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            }
            return string.Empty;
        }

        public static string GetIpAdress(this HttpRequest request)
        {
            if (request != null)
            {
                return GetIpAdress(request.HttpContext);
            }
            return string.Empty;
        }

        public static string GetIpAdressLogging(this HttpContext httpContext)
        {
            var ip = string.Empty;
            if (httpContext != null && httpContext.Request != null)
            {
                if (!string.IsNullOrEmpty(httpContext.Request.Headers[HeaderConstants.X_FORWARDED_FOR]))
                {
                    ip = httpContext.Request.Headers[HeaderConstants.X_FORWARDED_FOR];
                }
                else
                {
                    ip = httpContext.Request.HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString();
                }
            }
            return ip;
        }

        public static string GetIpAdress(this HttpContext httpContext)
        {
            var ip = string.Empty;
            if (httpContext != null && httpContext.Request != null)
            {
                if (!string.IsNullOrEmpty(httpContext.Request.Headers[HeaderConstants.X_FORWARDED_FOR]))
                {
                    ip = httpContext.Request.Headers[HeaderConstants.X_FORWARDED_FOR];
                }
                else
                {
                    ip = httpContext.Request.HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString();
                }
            }
            if (ip.Contains(","))
            {
                ip = ip.Split(",")[0];
            }
            if (IsValidIpV6(ip))
                return ip;
            else
                return ip.Split(":")[0].Split(",")[0];
        }

        public static bool IsValidIpV6(string ipAddress)
        {
            bool bResult = false;
            try
            {
                if (ipAddress.Contains(".") || ipAddress.Contains(":"))
                {
                    IPAddress address;
                    if (IPAddress.TryParse(ipAddress, out address))
                    {
                        switch (address.AddressFamily)
                        {
                            case AddressFamily.InterNetwork:
                                bResult = false;
                                break;

                            case AddressFamily.InterNetworkV6:
                                bResult = true;
                                break;
                        }
                    }
                }
            }
            catch
            {
                bResult = false;
            }
            return bResult;
        }

        public static string GetCultureCode(this HttpContext httpContext)
        {
            var cultureCode = string.Empty;
            if (httpContext != null && httpContext.Request != null)
            {
                if (httpContext.Request.Headers.ContainsKey(HeaderConstants.LOCALE))
                {
                    cultureCode = httpContext.Request.Headers[HeaderConstants.LOCALE];
                }

                if (string.IsNullOrEmpty(cultureCode) && httpContext.Request.Headers.ContainsKey(HeaderConstants.LOCALE.ToLower()))
                {
                    cultureCode = httpContext.Request.Headers[HeaderConstants.LOCALE.ToLower()];
                }
            }
            return cultureCode;
        }

        public static string GetOrigin(this HttpContext httpContext)
        {
            var origin = string.Empty;
            var httpRequest = httpContext?.Request;
            if (httpRequest != null)
            {
                if (!string.IsNullOrEmpty(httpRequest.Headers[HeaderConstants.X_FAKE_ORIGIN]))
                {
                    origin = httpRequest.Headers[HeaderConstants.X_FAKE_ORIGIN];
                }
                else if (!string.IsNullOrEmpty(httpRequest.Headers[HeaderConstants.ORIGIN]))
                {
                    origin = httpRequest.Headers[HeaderConstants.ORIGIN];
                }
            }
            return origin;
        }

        public static string GetAccessToken(this HttpContext context,
            JwtSettingModel jwtSettings, out bool onlyGetFromHeader)
        {
            onlyGetFromHeader = false;
            if (context != null)
            {
                return GetAccessToken(context.Request, jwtSettings, out onlyGetFromHeader);
            }
            return string.Empty;
        }

        public static string GetAccessToken(this MessageReceivedContext context,
            JwtSettingModel jwtSettings, out bool onlyGetFromHeader)
        {
            onlyGetFromHeader = false;
            if (context != null)
            {
                return GetAccessToken(context.Request, jwtSettings, out onlyGetFromHeader);
            }
            return string.Empty;
        }

        public static string GetAccessToken(this HttpRequest request,
            JwtSettingModel jwtSettings, out bool onlyGetFromHeader)
        {
            var jwtToken = string.Empty;
            onlyGetFromHeader = false;
            if (request != null)
            {
                if (request.Headers.ContainsKey(HeaderConstants.AUTHORIZATION))
                {
                    var bearerToken = request.Headers[HeaderConstants.AUTHORIZATION].FirstOrDefault();
                    if (!string.IsNullOrEmpty(bearerToken) && bearerToken.ToString().StartsWith(JwtBearerDefaults.AuthenticationScheme))
                    {
                        jwtToken = bearerToken.Substring($"{JwtBearerDefaults.AuthenticationScheme} ".Length).Trim();
                        onlyGetFromHeader = true;
                    }
                }

                if (!onlyGetFromHeader && string.IsNullOrEmpty(jwtToken) && request.Cookies.ContainsKey(jwtSettings.AccessTokenName))
                {
                    jwtToken = request.Cookies[jwtSettings.AccessTokenName];
                }
            }
            return jwtToken;
        }

        public static long GetStartRange(this IHttpContextAccessor httpContextAccessor)
        {
            return GetStartRange(httpContextAccessor.HttpContext); ;
        }

        public static long GetStartRange(this HttpContext httpContext)
        {
            long startRange = 0;
            var requestHeaders = httpContext?.Request.Headers;
            if (requestHeaders != null && requestHeaders.ContainsKey(HeaderConstants.RANGE))
            {
                var rangeValue = requestHeaders[HeaderConstants.RANGE].ToString()?.Replace("bytes=", string.Empty);
                if (!string.IsNullOrEmpty(rangeValue) && rangeValue.Contains("-"))
                {
                    long.TryParse(rangeValue.Split('-')[0], out startRange);
                }
            }
            return startRange;
        }

        public static Guid GetFolderContentId(this HttpContext httpContext)
        {
            var requestHeaders = httpContext?.Request.Headers;
            if (requestHeaders != null && requestHeaders.ContainsKey(HeaderConstants.X_FOLDER_CONTENT_ID))
            {
                var folderContentIdString = requestHeaders[HeaderConstants.X_FOLDER_CONTENT_ID].ToString();
                if (!string.IsNullOrEmpty(folderContentIdString))
                {
                    if (Guid.TryParse(folderContentIdString, out var folderContentId))
                    {
                        return folderContentId;
                    }
                }
            }
            return Guid.Empty;
        }

        public static string GetAccessToken(this MessageReceivedContext context, JwtSettingModel jwtSettings)
        {
            if (context != null)
            {
                return GetAccessToken(context.Request, jwtSettings, out _);
            }
            return string.Empty;
        }
    }
}
