using HealthChecks.UI.Client;
using IEM.Application.Extensions;
using IEM.Application.HealthCheck;
using IEM.Application.Middlewares;
using IEM.Infrastructure.Extensions;
using IEM.WebAPI.Extensions;
using IEM.WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var appSettings = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .Build();

builder.Services.AddJwtAuthentication(appSettings);
builder.Services.AddApplicationSevices(appSettings);
builder.Services.AddApplicationDbContext(appSettings);

builder.Services.AddRepositoryServices();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddCors(appSettings);
builder.Services.AddHealthChecks()
    .AddCheck<SqlServerHealthCheck>("SqlServer");
builder.Services.AddHealthChecksUI(options =>
{
    options.AddHealthCheckEndpoint("Healthcheck API", "/healthcheck");
})
    .AddInMemoryStorage();

builder.Services.AddWebApiService(appSettings);

var app = builder.Build();

app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseMiddleware<ExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthcheck", new()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
})
    .AllowAnonymous();

app.MapHealthChecksUI(options => options.UIPath = "/dashboard")
    .AllowAnonymous();

app.Run();
