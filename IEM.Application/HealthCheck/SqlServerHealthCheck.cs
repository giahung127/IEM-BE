using IEM.Application.Models.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEM.Application.HealthCheck
{
    public class SqlServerHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;
        public SqlServerHealthCheck(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(AppSettingConstants.CONNECTION_STRING)!;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using var connection = new SqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync(cancellationToken);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception:ex);
            }
        }
    }
}
