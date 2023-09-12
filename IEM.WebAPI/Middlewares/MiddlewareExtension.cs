using Hangfire.Dashboard;
using Hangfire;
using IEM.Application.Models.Constants;
using IEM.Application.Models.Settings;
using IEM.Infrastructure.Hangfire;
using HealthChecks.UI.Client;

namespace IEM.WebAPI.Middlewares
{
    public static class MiddlewareExtension
    {
        public static void UseEndpoints(this IApplicationBuilder app, IConfiguration configuration)
        {
            var jobDashboardSetting = configuration.GetSection(AppSettingConstants.BACKGROUND_JOB).Get<BackgroundJobSettingModel>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                if (jobDashboardSetting != null && jobDashboardSetting.Dashboard != null && jobDashboardSetting.DashboardEnabled)
                {
                    var path = jobDashboardSetting.Dashboard.Path?.TrimStart('/').TrimEnd('/') ?? "hangfire";
                    path = "/" + path;
                    app.UseHangfireDashboard(path, new DashboardOptions
                    {
                        IsReadOnlyFunc = (DashboardContext context) => jobDashboardSetting.Dashboard.IsReadOnly,
                        Authorization = new[] { new HangFireAuthorizationFilter() }
                    });
                    endpoints.MapHangfireDashboard().AllowAnonymous();
                }

                endpoints.MapHealthChecks("/healthcheck", new()
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                }).AllowAnonymous();

                endpoints.MapHealthChecksUI(options => options.UIPath = "/dashboard").AllowAnonymous();
            });
        }
    }
}
