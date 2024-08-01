using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Authorization;

public class IsOwnerAuthorizationHandler(UserManager<User> userManager)
    : AuthorizationHandler<IsOwnerAuthorizationRequirement, Tournament>
{
    readonly UserManager<User> userManager = userManager;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext authorizationContext,
        IsOwnerAuthorizationRequirement requirement,
        Tournament resource
    )
    {
        if (authorizationContext.User == null || resource == null)
        {
            return Task.CompletedTask;
        }

        if (resource.OwnerId == userManager.GetUserId(authorizationContext.User))
        {
            authorizationContext.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class IsOwnerAuthorizationRequirement : IAuthorizationRequirement { }
