using IEM.Application.Models.Constants;
using Microsoft.AspNetCore.Authorization;

namespace IEM.Application.Authorizations
{
    internal class AdminAccessHandler : AuthorizationHandler<AdminAccessRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminAccessRequirement requirement)
        {
            var adminAccess = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypeConstants.ADMIN_ACCESS).Value;
            if (adminAccess == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.AdminRoles.Contains(int.Parse(adminAccess)))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
