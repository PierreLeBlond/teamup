using Microsoft.AspNetCore.Identity;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class GamePageModel(ApplicationDbContext context, UserManager<User> userManager)
    : TournamentPageModel(context, userManager)
{
    public Game Game { get; set; } = null!;

    protected virtual void SetModel(string tournamentId, string gameId, string? currentPlayerId)
    {
        base.SetModel(tournamentId, currentPlayerId);

        var gameGuid = new Guid(gameId);
        Game = context.Games.Single(g => g.Id == gameGuid);
    }
}
