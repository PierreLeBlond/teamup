using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Webapp.Pages.Components;

public class NavigationViewComponentModel
{
    public string? TournamentId { get; set; } = null;
    public string? GameId { get; set; } = null;
    public string? TeamId { get; set; } = null;
    public string? CurrentPlayerId { get; set; } = null;
}

public class NavigationViewComponent() : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var tournamentId = HttpContext.Request.RouteValues["tournamentId"];
        var gameId = HttpContext.Request.RouteValues["gameId"];
        var teamId = HttpContext.Request.RouteValues["teamId"];
        var hasCurrentPlayer = HttpContext.Request.Query.TryGetValue(
            "currentPlayerId",
            out StringValues currentPlayerId
        );

        var model = new NavigationViewComponentModel
        {
            TournamentId = tournamentId is null ? null : (string)tournamentId,
            GameId = gameId is null ? null : (string)gameId,
            TeamId = teamId is null ? null : (string)teamId,
            CurrentPlayerId = hasCurrentPlayer ? currentPlayerId.ToString() : null
        };

        return View(model);
    }
}
