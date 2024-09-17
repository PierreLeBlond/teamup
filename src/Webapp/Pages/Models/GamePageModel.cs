using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class GamePageModel(ApplicationDbContext context, UserManager<User> userManager)
    : TournamentPageModel(context, userManager)
{
    public Game Game { get; set; } = null!;

    public Team? CurrentTeam { get; set; } = null;

    protected virtual void SetModel(string tournamentId, string gameId, string? currentPlayerId)
    {
        base.SetModel(tournamentId, currentPlayerId);

        var gameGuid = new Guid(gameId);
        Game = Tournament.Games.Single(g => g.Id == gameGuid);

        CurrentTeam = context.GetCurrentTeam(Game, CurrentPlayer);
    }

    protected RedirectResult RedirectToGames()
    {
        return Redirect($"/tournaments/{Tournament.Id}/games/{Game.Id}{GetQueryString()}");
    }
}
