using AutoMapper;
using IEM.Application.Models.Auth;
using IEM.Application.Models.Constants;
using IEM.Application.Models.Settings;
using IEM.Application.Utils;
using IEM.Domain.Core.Repositories;
using IEM.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace IEM.Application.Services
{
    internal class BaseService
    {
        protected readonly IConfiguration Configuration;
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IMapper Mapper;
        protected readonly ILogger Logger;
        private readonly IHttpContextAccessor HttpContextAccessor;

        protected JwtSettingModel JwtSettings
        {
            get
            {
                return Configuration.GetSection(AppSettingConstants.JWT).Get<JwtSettingModel>()!;
            }
        }

        public BaseService(IServiceProvider provider, ILogger logger)
        {
            UnitOfWork = provider.GetService<IUnitOfWork>()!;
            Mapper = provider.GetService<IMapper>()!;
            Configuration = provider.GetService<IConfiguration>()!;
            HttpContextAccessor = provider.GetService<IHttpContextAccessor>()!;
            Logger = logger;
        }

        protected JwtSecurityToken? ParseJwtSecurityToken(TokenTypes type, JwtSettingModel jwtSettings, string token)
        {
            return TokenUtils.ParseJwtSecurityToken(type, jwtSettings, token, Logger);
        }

        public async ValueTask<TokenValidationModel> ValidateAccessTokenAsync(string accessToken)
        {
            var result = new TokenValidationModel();
            var jwtSecurityToken = ParseJwtSecurityToken(TokenTypes.AccessToken, JwtSettings, accessToken);
            if (jwtSecurityToken != null)
            {
                var userId = Guid.Parse(jwtSecurityToken.Claims.First(x => x.Type == ClaimTypeConstants.USER_ID).Value);
                var domain = await this.UnitOfWork.UserConnections
                            .FirstOrDefaultAsync(i => i.AccessToken == accessToken && i.UserId == userId
                                                   && i.AccessTokenExpiredDate >= DateTimeOffset.UtcNow);
                result.IsValid = domain != null;
                result.UserId = userId;
            }
            return result;
        }

        public TokenValidationModel ValidateAccessToken(string accessToken)
        {
            return ValidateAccessTokenAsync(accessToken).Result;
        }
    }
}
