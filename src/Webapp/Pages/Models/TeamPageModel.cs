using Microsoft.AspNetCore.Identity;
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

        var teamGuid = new Guid(teamId);
        Team = context.Teams.Include(t => t.Result).Single(t => t.Id == teamGuid);

        Team.Score = context.GetTeamScore(Game, Team);
    }
}
