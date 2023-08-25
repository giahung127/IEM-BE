using IEM.Application.Attributes;
using IEM.Application.Models.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace IEM.Application.Swaggers
{
    public class AddSecurityHeaderParameterFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //https://stackoverflow.com/questions/41493130/web-api-how-to-add-a-header-parameter-for-all-api-in-swagger
            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                var apiAttributes = context.ApiDescription.CustomAttributes();
                var controllerTypeInfo = descriptor.ControllerTypeInfo;
                var hasAnonymous = apiAttributes.Any((a) => a is AllowAnonymousAttribute);

                if ((!hasAnonymous || apiAttributes.Any((a) => a is SwaggerApiSecurityAttribute))
                    && (apiAttributes.Any((a) => a is AuthorizeAttribute) || controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>() != null))
                {
                    if (operation.Security == null)
                    {
                        operation.Security = new List<OpenApiSecurityRequirement>();
                    }

                    var securityRequirement = new OpenApiSecurityScheme
                    {
                        Name = HeaderConstants.AUTHORIZATION,
                        In = ParameterLocation.Header,
                        BearerFormat = $"{JwtBearerDefaults.AuthenticationScheme} token",
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    };
                    operation.Security.Add(new OpenApiSecurityRequirement {
                        { securityRequirement, new string[] { JwtBearerDefaults.AuthenticationScheme } }
                    });
                }
            }
        }
    }
}
