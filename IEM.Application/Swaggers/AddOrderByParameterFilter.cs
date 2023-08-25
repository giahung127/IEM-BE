using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IEM.Application.Swaggers
{
    public class AddOrderByParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (!parameter.In.HasValue || parameter.In.Value != ParameterLocation.Query)
                return;

            if (parameter.Schema?.Type == "object" && parameter.Name.Equals("orderBy"))
            {
                parameter.Style = ParameterStyle.DeepObject;
                parameter.Explode = true;
            }
        }
    }
}
