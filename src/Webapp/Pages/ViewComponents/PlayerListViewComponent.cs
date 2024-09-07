using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages.ViewComponents;

public class PlayerListViewComponent(ApplicationDbContext context) : ViewComponent
{
    private readonly ApplicationDbContext context = context;

    public async Task<IViewComponentResult> InvokeAsync(Guid tournamentId)
    {
        var players = await GetPlayersAsync(tournamentId);
        return View(players);
    }

    private Task<List<Player>> GetPlayersAsync(Guid tournamentId)
    {
        return context.Players.Where(p => p.TournamentId == tournamentId).ToListAsync();
    }
}
