using IEM.Application.Models.Constants;
using IEM.Application.Models.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IEM.Application.Swaggers
{
    public class AddOriginHeaderParameterFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.CustomAttributes().Any((a) => a is AllowAnonymousAttribute))
            {
                operation.Parameters ??= new List<OpenApiParameter>();
                var existingParam = operation.Parameters.FirstOrDefault(p => p.In == ParameterLocation.Header
                                    && p.Name.EqualsIgnoreCase(HeaderConstants.X_FAKE_ORIGIN));
                if (existingParam != null)
                {
                    operation.Parameters.Remove(existingParam);
                }

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = HeaderConstants.X_FAKE_ORIGIN,
                    In = ParameterLocation.Header,
                    Description = "Use the X-Fake-Origin header for Swagger (the Origin header in the real) to identify the entity in case of API allows anonymous access",
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "String",
                        Default = new OpenApiString("")
                    }
                });
            }
        }
    }
}
