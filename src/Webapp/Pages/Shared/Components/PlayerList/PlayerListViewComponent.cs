using Microsoft.AspNetCore.Mvc;
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

    public IViewComponentResult Invoke()
    {
        var tournamentId = HttpContext.Request.RouteValues["tournamentId"];
        if (tournamentId is null)
        {
            return Content(string.Empty);
        }

        var tournamentGuid = Guid.Parse((string)tournamentId);
        var tournament = context.GetTournament(tournamentGuid);

        var players = tournament.Players.ToList();
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
        model.Player = players.Single(p => p.Id == currentPlayerGuid);

        return View(model);
    }
}
