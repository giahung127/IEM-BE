using IEM.Application.Models.Constants;
using IEM.Domain.Core.Repositories;
using IEM.Infrastructure.Data;
using IEM.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IEM.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
        }

        public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDatabaseContext>((options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString(AppSettingConstants.CONNECTION_STRING), sqlServerOptions => sqlServerOptions.CommandTimeout(120));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableSensitiveDataLogging();
            }, ServiceLifetime.Scoped);
        }

        public static void MigrateDatabase(IServiceProvider serviceProvider)
        {
            var dbContextOptions = serviceProvider.GetRequiredService<DbContextOptions<ApplicationDatabaseContext>>();

            using (var dbContext = new ApplicationDatabaseContext(dbContextOptions))
            {
                dbContext.Database.Migrate();
            }
        }
    }
}
