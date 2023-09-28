using Microsoft.AspNetCore.Authorization;

namespace IEM.Application.Authorizations
{
    public class AdminAccessRequirement : IAuthorizationRequirement
    {
        public readonly List<int> AdminRoles;
        public AdminAccessRequirement(params int[] adminRoles)
        {
            AdminRoles = new List<int>() { };
            if (adminRoles != null)
            {
                AdminRoles.AddRange(adminRoles);
            }
        }
    }
}
