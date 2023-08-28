using AutoMapper;
using IEM.Application.AutoMapperProfile;
using IEM.Application.Interfaces;
using IEM.Application.Models.Constants;
using IEM.Application.Models.Exceptions;
using IEM.Application.Models.Extensions;
using IEM.Application.Services;
using IEM.Application.Swaggers;
using IEM.Application.Utils;
using IEM.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace IEM.Application.Extensions
{
    public static class ServiceExtensions
    {

        public static void AddCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options => options.AddDefaultPolicy((builder) =>
            {
                var originConfig = configuration.GetSection(AppSettingConstants.CORS_ORIGINS).Get<string>();
                if (string.IsNullOrEmpty(originConfig) || originConfig.Equals("*"))
                {
                    builder
                        .SetIsOriginAllowed((_) => true)
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders(HeaderConstants.CONTENT_DISPOSITION)
                        .SetPreflightMaxAge(TimeSpan.FromDays(30));
                }
                else
                {
                    // CorsOrigins in appsettings.json can contain more than one address separated by semicolon.
                    var origins = originConfig.Split(";", StringSplitOptions.RemoveEmptyEntries)
                          .Select(o => o.RemovePostFix("/")).ToArray();
                    builder.WithOrigins(origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders(HeaderConstants.CONTENT_DISPOSITION)
                        .SetPreflightMaxAge(TimeSpan.FromDays(30));
                }
            }));
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "IEM Api",
                    Version = "v1",
                });
                opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
                {
                    Name = HeaderConstants.AUTHORIZATION,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = $"{JwtBearerDefaults.AuthenticationScheme} token",
                    In = ParameterLocation.Header,
                    Description = $"Use the {HeaderConstants.AUTHORIZATION} header to authenticate allowing access to a protected API",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                });
                opt.OperationFilter<AddSecurityHeaderParameterFilter>();
                opt.ResolveConflictingActions(ApiDescriptionConflictResolver.Resolve);
                opt.SwaggerGeneratorOptions.DescribeAllParametersInCamelCase = true;
                opt.ParameterFilter<AddOrderByParameterFilter>();
                opt.OperationFilter<AddOriginHeaderParameterFilter>();
                //opt.OperationFilter<ApplySwaggerDocFilter>();
                //opt.OperationFilter<AddMultipartFormDataOperationFilter>();
            });
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = TokenUtils.GetJwtSettings(configuration);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = TokenUtils.CreateJwtTokenParameters(TokenTypes.AccessToken, jwtSettings);
                options.SaveToken = true;
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        if (context.AuthenticateFailure != null)
                        {
                            switch (context.AuthenticateFailure)
                            {
                                case DomainException e:
                                    return context.CreateJsonResponseAsync(StatusCodes.Status400BadRequest, e.ErrorCode, e.Message);

                                case HttpException e:
                                    return context.CreateJsonResponseAsync((int)e.StatusCode, (int)e.StatusCode, e.Message);
                            }
                        }
                        return Task.CompletedTask;
                    },
                    //OnTokenValidated = context =>
                    //{
                    //    var claimPrincipal = context.Principal;
                    //    var userId = TokenUtils.GetUserId(claimPrincipal);
                    //    var httpContext = context.HttpContext;
                    //    var userService = httpContext.RequestServices.GetRequiredService<IUserService>();
                    //    var userContextProvider = httpContext.RequestServices.GetRequiredService<IJwtUserContextProvider>();
                    //    var user = userService.GetById(userId, entityId);

                    //    var entityDomainSettings = TokenUtils.GetEntityDomainSettings(configuration);
                    //    var deviceType = HttpContextUtils.GetDeviceType(httpContext, entityDomainSettings);
                    //    var isBackOffice = deviceType == DeviceTypes.DBJ_BACK_OFFICE
                    //                       || deviceType == DeviceTypes.IOD_BACK_OFFICE
                    //                       || deviceType == DeviceTypes.BACK_OFFICE;
                    //    if (user == null)
                    //    {
                    //        context.Fail(new HttpUnauthorizedException());
                    //    }
                    //    else if (!isBackOffice && !user.IsAcceptTermsOfUse)
                    //    {
                    //        context.Fail(new UserTermsOfUseNotAcceptedException());
                    //    }
                    //    else
                    //    {
                    //        var authService = httpContext.RequestServices.GetRequiredService<IAuthService>();
                    //        userContextProvider.EnabledSSO = authService.EnableSSO(user.Email);
                    //        UserUtils.SetUserContext(userContextProvider, user, authService.EnableSSO(user.Email));
                    //    }

                    //    return Task.CompletedTask;
                    //},
                    OnMessageReceived = context =>
                    {
                        var endpoint = context.HttpContext.GetEndpoint();
                        var authorized = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() == null;
                        if (authorized)
                        {
                            var accessToken = context.GetAccessToken(jwtSettings);
                            var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
                            var validationResult = authService.ValidateAccessToken(accessToken);

                            if (validationResult.IsValid)
                            {
                                context.Token = accessToken;
                            }
                            else
                            {
                                throw new SecurityTokenValidationException();
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        }

        public static void AddApplicationSevices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Core Services
            services.AddAutoMapper(configuration);
            #endregion

            #region Bussiness Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            #endregion
        }

        #region private
        private static void AddAutoMapper(this IServiceCollection services, IConfiguration configuration)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserMapperProfile(configuration));
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
        #endregion
    }
}
