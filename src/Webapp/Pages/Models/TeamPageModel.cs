using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class TeamPageModel(ApplicationDbContext context, UserManager<User> userManager)
    : GamePageModel(context, userManager)
{
    public Team Team { get; set; } = null!;

    protected virtual void SetModel(
        string tournamentId,
        string gameId,
        string teamId,
        string? currentPlayerId
    )
    {
        base.SetModel(tournamentId, gameId, currentPlayerId);

        Team = Game.Teams.Single(t => t.Id == int.Parse(teamId));
    }

    protected RedirectResult RedirectToTeams()
    {
        return Redirect(
            $"/tournaments/{Tournament.Id}/games/{Game.Id}/teams/{Team.Id}{GetQueryString()}"
        );
    }
}
