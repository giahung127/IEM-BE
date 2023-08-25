using IEM.Application.Models.Constants;
using IEM.Application.Models.Settings;
using IEM.Application.Models.Users;
using IEM.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IEM.Application.Utils
{
    public static class TokenUtils
    {
        public static IConfigurationSection GetJwtSettingsConfiguration(IConfiguration configuration)
        {
            return configuration.GetRequiredSection(AppSettingConstants.JWT);
        }

        public static JwtSettingModel GetJwtSettings(IConfiguration configuration)
        {
            return GetJwtSettingsConfiguration(configuration).Get<JwtSettingModel>();
        }

        public static TokenValidationParameters CreateJwtTokenParameters(TokenTypes type, JwtSettingModel jwtSettings)
        {
            string secretToken = jwtSettings.AccessTokenSecret;
            switch (type)
            {
                case TokenTypes.RefreshToken:
                    secretToken = jwtSettings.RefreshTokenSecret;
                    break;
            }
            return new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretToken)),
                ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                ValidateIssuer = jwtSettings.ValidateIssuer,
                ValidateAudience = jwtSettings.ValidateAudience,
                ValidateLifetime = jwtSettings.ValidateLifetime,
                RequireExpirationTime = jwtSettings.RequireExpirationTime,
                RequireSignedTokens = jwtSettings.RequireSignedTokens,
                RequireAudience = jwtSettings.RequireAudience,
                ValidIssuer = jwtSettings.ValidIssuer,
                ValidAudience = jwtSettings.ValidAudience,
                ClockSkew = TimeSpan.Zero
            };
        }

        public static Tuple<string, DateTimeOffset> CreateJwtRefreshToken(JwtSettingModel jwtSettings, UserTokenModel user)
        {
            return CreateJwtToken(TokenTypes.RefreshToken, jwtSettings, user);
        }

        public static Tuple<string, DateTimeOffset> CreateJwtAccessToken(JwtSettingModel jwtSettings, UserTokenModel user)
        {
            return CreateJwtToken(TokenTypes.AccessToken, jwtSettings, user);
        }

        public static JwtSecurityToken ParseJwtSecurityToken(TokenTypes type, JwtSettingModel jwtSettings, string token, ILogger logger)
        {
            SecurityToken validatedToken = null;
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, CreateJwtTokenParameters(type, jwtSettings), out validatedToken);
            }
            catch (SecurityTokenExpiredException ex)
            {
                logger.LogWarning(ex.Message);
                throw new SecurityTokenExpiredException();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.Message);
            }

            return validatedToken != null ? (JwtSecurityToken)validatedToken : null;
        }

        #region private

        private static Tuple<string, DateTimeOffset> CreateJwtToken(TokenTypes type, JwtSettingModel jwtSettings, UserTokenModel user)
        {
            double expirationDays = jwtSettings.AccessTokenExpirationDays;
            string secretToken = jwtSettings.AccessTokenSecret;
            switch (type)
            {
                case TokenTypes.RefreshToken:
                    secretToken = jwtSettings.RefreshTokenSecret;
                    expirationDays = jwtSettings.RefreshTokenExpirationDays;
                    break;
            }
            Enum.TryParse<JwtSecurityAlgorithms>(jwtSettings.SecurityAlgorithm, out var algorithm);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretToken));
            var signingCredentials = new SigningCredentials(securityKey, GetSecurityAlgorithms(algorithm));
            var expiredDate = DateTimeOffset.UtcNow.AddDays(expirationDays);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypeConstants.ADMIN_ACCESS, $"{user.AdminAccess}")
            };

            if (user.UserId != null && user.UserId != Guid.Empty)
            {
                claims.Add(new Claim(ClaimTypeConstants.USER_ID, user.UserId.ToString()));
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.Email));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.UtcNow,
                Issuer = jwtSettings.ValidIssuer,
                Audience = jwtSettings.ValidAudience,
                Expires = expiredDate.DateTime,
                SigningCredentials = signingCredentials,
            };
            if (!string.IsNullOrEmpty(user.Audience))
            {
                tokenDescriptor.Audience = user.Audience;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Tuple.Create(tokenHandler.WriteToken(token), expiredDate);
        }

        private static string GetSecurityAlgorithms(JwtSecurityAlgorithms algorithm)
        {
            switch (algorithm)
            {
                case JwtSecurityAlgorithms.Aes128Encryption:
                    return SecurityAlgorithms.Aes128Encryption;

                case JwtSecurityAlgorithms.Aes192Encryption:
                    return SecurityAlgorithms.Aes192Encryption;

                case JwtSecurityAlgorithms.Aes256Encryption:
                    return SecurityAlgorithms.Aes256Encryption;

                case JwtSecurityAlgorithms.DesEncryption:
                    return SecurityAlgorithms.DesEncryption;

                case JwtSecurityAlgorithms.Aes128KeyWrap:
                    return SecurityAlgorithms.Aes128KeyWrap;

                case JwtSecurityAlgorithms.Aes192KeyWrap:
                    return SecurityAlgorithms.Aes192KeyWrap;

                case JwtSecurityAlgorithms.Aes256KeyWrap:
                    return SecurityAlgorithms.Aes256KeyWrap;

                case JwtSecurityAlgorithms.RsaV15KeyWrap:
                    return SecurityAlgorithms.RsaV15KeyWrap;

                case JwtSecurityAlgorithms.Ripemd160Digest:
                    return SecurityAlgorithms.Ripemd160Digest;

                case JwtSecurityAlgorithms.RsaOaepKeyWrap:
                    return SecurityAlgorithms.RsaOaepKeyWrap;

                case JwtSecurityAlgorithms.Aes128KW:
                    return SecurityAlgorithms.Aes128KW;

                case JwtSecurityAlgorithms.Aes256KW:
                    return SecurityAlgorithms.Aes256KW;

                case JwtSecurityAlgorithms.RsaPKCS1:
                    return SecurityAlgorithms.RsaPKCS1;

                case JwtSecurityAlgorithms.RsaOAEP:
                    return SecurityAlgorithms.RsaOAEP;

                case JwtSecurityAlgorithms.ExclusiveC14n:
                    return SecurityAlgorithms.ExclusiveC14n;

                case JwtSecurityAlgorithms.ExclusiveC14nWithComments:
                    return SecurityAlgorithms.ExclusiveC14nWithComments;

                case JwtSecurityAlgorithms.EnvelopedSignature:
                    return SecurityAlgorithms.EnvelopedSignature;

                case JwtSecurityAlgorithms.Sha256Digest:
                    return SecurityAlgorithms.Sha256Digest;

                case JwtSecurityAlgorithms.Sha384Digest:
                    return SecurityAlgorithms.Sha384Digest;

                case JwtSecurityAlgorithms.Sha512Digest:
                    return SecurityAlgorithms.Sha512Digest;

                case JwtSecurityAlgorithms.Sha256:
                    return SecurityAlgorithms.Sha256;

                case JwtSecurityAlgorithms.Sha384:
                    return SecurityAlgorithms.Sha384;

                case JwtSecurityAlgorithms.Sha512:
                    return SecurityAlgorithms.Sha512;

                case JwtSecurityAlgorithms.EcdsaSha256Signature:
                    return SecurityAlgorithms.EcdsaSha256Signature;

                case JwtSecurityAlgorithms.EcdsaSha384Signature:
                    return SecurityAlgorithms.EcdsaSha384Signature;

                case JwtSecurityAlgorithms.EcdsaSha512Signature:
                    return SecurityAlgorithms.EcdsaSha512Signature;

                case JwtSecurityAlgorithms.HmacSha256Signature:
                    return SecurityAlgorithms.HmacSha256Signature;

                case JwtSecurityAlgorithms.HmacSha384Signature:
                    return SecurityAlgorithms.HmacSha384Signature;

                case JwtSecurityAlgorithms.HmacSha512Signature:
                    return SecurityAlgorithms.HmacSha512Signature;

                case JwtSecurityAlgorithms.RsaSha256Signature:
                    return SecurityAlgorithms.RsaSha256Signature;

                case JwtSecurityAlgorithms.RsaSha384Signature:
                    return SecurityAlgorithms.RsaSha384Signature;

                case JwtSecurityAlgorithms.RsaSha512Signature:
                    return SecurityAlgorithms.RsaSha512Signature;

                case JwtSecurityAlgorithms.RsaSsaPssSha256Signature:
                    return SecurityAlgorithms.RsaSsaPssSha256Signature;

                case JwtSecurityAlgorithms.RsaSsaPssSha384Signature:
                    return SecurityAlgorithms.RsaSsaPssSha384Signature;

                case JwtSecurityAlgorithms.RsaSsaPssSha512Signature:
                    return SecurityAlgorithms.RsaSsaPssSha512Signature;

                case JwtSecurityAlgorithms.EcdsaSha256:
                    return SecurityAlgorithms.EcdsaSha256;

                case JwtSecurityAlgorithms.EcdsaSha384:
                    return SecurityAlgorithms.EcdsaSha384;

                case JwtSecurityAlgorithms.EcdsaSha512:
                    return SecurityAlgorithms.EcdsaSha512;

                case JwtSecurityAlgorithms.HmacSha256:
                    return SecurityAlgorithms.HmacSha256;

                case JwtSecurityAlgorithms.HmacSha384:
                    return SecurityAlgorithms.HmacSha384;

                case JwtSecurityAlgorithms.HmacSha512:
                    return SecurityAlgorithms.HmacSha512;

                case JwtSecurityAlgorithms.RsaSha256:
                    return SecurityAlgorithms.RsaSha256;

                case JwtSecurityAlgorithms.RsaSha384:
                    return SecurityAlgorithms.RsaSha384;

                case JwtSecurityAlgorithms.RsaSha512:
                    return SecurityAlgorithms.RsaSha512;

                case JwtSecurityAlgorithms.RsaSsaPssSha256:
                    return SecurityAlgorithms.RsaSsaPssSha256;

                case JwtSecurityAlgorithms.RsaSsaPssSha384:
                    return SecurityAlgorithms.RsaSsaPssSha384;

                case JwtSecurityAlgorithms.RsaSsaPssSha512:
                    return SecurityAlgorithms.RsaSsaPssSha512;

                case JwtSecurityAlgorithms.Aes128CbcHmacSha256:
                    return SecurityAlgorithms.Aes128CbcHmacSha256;

                case JwtSecurityAlgorithms.Aes192CbcHmacSha384:
                    return SecurityAlgorithms.Aes192CbcHmacSha384;

                case JwtSecurityAlgorithms.Aes256CbcHmacSha512:
                    return SecurityAlgorithms.Aes256CbcHmacSha512;
            }
            return SecurityAlgorithms.HmacSha256;
        }

        #endregion private
    }
}
