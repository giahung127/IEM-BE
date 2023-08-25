using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace IEM.Application.Swaggers
{
    public static class ApiDescriptionConflictResolver
    {
        public static ApiDescription Resolve(IEnumerable<ApiDescription> descriptions)
        {
            return descriptions.First();
        }
    }
}
