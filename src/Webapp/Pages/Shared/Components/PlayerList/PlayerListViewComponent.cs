using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages.Components;

public class PlayerListViewComponentModel
{
    public IEnumerable<Player> Players { get; set; } = [];
    public Player? Player { get; set; } = null;
}

public class PlayerListViewComponent(ApplicationDbContext context) : ViewComponent
{
    private readonly ApplicationDbContext context = context;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var tournamentId = HttpContext.Request.RouteValues["tournamentId"];
        if (tournamentId is null)
        {
            return Content(string.Empty);
        }

        var tournamentGuid = Guid.Parse((string)tournamentId);

        var players = await GetPlayersAsync(tournamentGuid);
        var model = new PlayerListViewComponentModel { Players = players };

        if (
            !HttpContext.Request.Query.TryGetValue(
                "currentPlayerId",
                out StringValues currentPlayerId
            )
        )
        {
            return View(model);
        }

        var currentPlayerGuid = Guid.Parse(currentPlayerId.ToString());
        model.Player = context.Players.Single(p => p.Id == currentPlayerGuid);

        var tournament = context.Tournaments.Single(t => t.Id == tournamentGuid);
        model.Player.Score = context.GetPlayerScore(tournament, currentPlayerGuid);

        return View(model);
    }

    private Task<List<Player>> GetPlayersAsync(Guid tournamentId)
    {
        return context.Players.Where(p => p.TournamentId == tournamentId).ToListAsync();
    }
}
