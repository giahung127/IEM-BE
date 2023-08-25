using IEM.Application.Models.Constants;
using IEM.Application.Models.Settings;

namespace IEM.WebAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
        }

        public static void AddWebApiService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ResponseTimeSettingModel>()
                .Bind(configuration.GetSection(AppSettingConstants.MEASURING_RESPONSE_TIME));

            services.AddOptions<SecurityHeaderSettingModel>()
                .Bind(configuration.GetSection(AppSettingConstants.SECURITY_HEADERS));

            services.AddOptions<HttpLoggingSettingModel>()
              .Bind(configuration.GetSection(AppSettingConstants.HTTP_LOGGING));


        }
    }
}
