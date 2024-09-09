using Microsoft.AspNetCore.Identity;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class TeamPageModel(ApplicationDbContext context, UserManager<User> userManager)
    : GamePageModel(context, userManager)
{
    public Team Team { get; set; } = null!;

    public int Score { get; set; } = 0;

    protected virtual void SetModel(
        string tournamentId,
        string gameId,
        string teamId,
        string? currentPlayerId
    )
    {
        base.SetModel(tournamentId, gameId, currentPlayerId);

        var teamGuid = new Guid(teamId);
        Team = context.Teams.Single(t => t.Id == teamGuid);

        Score = context.GetTeamScore(Game, Team);
    }
}
