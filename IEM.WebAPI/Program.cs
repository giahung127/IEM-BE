using IEM.Application.Middlewares;
using IEM.WebAPI.Extensions;
using IEM.WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var appSettings = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .Build();


IEM.Application.Extensions.ServiceExtensions.AddJwtAuthentication(builder.Services, appSettings);
IEM.Application.Extensions.ServiceExtensions.AddApplicationSevices(builder.Services, appSettings);
IEM.Infrastructure.Extensions.ServiceExtensions.AddApplicationDbContext(builder.Services, appSettings);
IEM.Infrastructure.Extensions.ServiceExtensions.AddRepositoryServices(builder.Services);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
IEM.Application.Extensions.ServiceExtensions.AddSwagger(builder.Services);
IEM.Application.Extensions.ServiceExtensions.AddCors(builder.Services, appSettings);


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

app.Run();
