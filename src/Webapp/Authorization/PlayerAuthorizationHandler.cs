using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Authorization;

public class PlayerAuthorizationHandler(Context context, UserManager<User> userManager)
    : AuthorizationHandler<PlayerAuthorizationRequirement, Player>
{
    readonly UserManager<User> userManager = userManager;
    readonly Context context = context;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext authorizationContext,
        PlayerAuthorizationRequirement requirement,
        Player resource
    )
    {
        if (authorizationContext.User == null || resource == null)
        {
            return Task.CompletedTask;
        }

        var tournament = context.Tournaments.Single(tournament =>
            tournament.Name == resource.TournamentId
        );

        if (tournament.OwnerId == userManager.GetUserId(authorizationContext.User))
        {
            authorizationContext.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class PlayerAuthorizationRequirement : IAuthorizationRequirement { }
