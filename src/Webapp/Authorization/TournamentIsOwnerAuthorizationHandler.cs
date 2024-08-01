using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Webapp.Models;

namespace Webapp.Authorization;

public class TournamentIsOwnerAuthorizationHandler(UserManager<User> userManager)
    : AuthorizationHandler<OperationAuthorizationRequirement, Tournament>
{
    readonly UserManager<User> userManager = userManager;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Tournament resource
    )
    {
        if (context.User == null || resource == null)
        {
            return Task.CompletedTask;
        }

        // If not asking for CRUD permission, return.

        if (
            requirement.Name != Constants.CreateOperationName
            && requirement.Name != Constants.ReadOperationName
            && requirement.Name != Constants.UpdateOperationName
            && requirement.Name != Constants.DeleteOperationName
        )
        {
            return Task.CompletedTask;
        }

        if (resource.OwnerId == userManager.GetUserId(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
