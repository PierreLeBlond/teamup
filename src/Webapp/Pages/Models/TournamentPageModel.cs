using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class TournamentPageModel(ApplicationDbContext context, UserManager<User> userManager)
    : PageModel
{
    protected readonly ApplicationDbContext context = context;
    protected readonly UserManager<User> userManager = userManager;

    public Tournament Tournament { get; set; } = null!;
    public Player? CurrentPlayer { get; set; } = null;
    public bool IsOwner { get; set; } = false;

    protected virtual void SetModel(string tournamentId, string? currentPlayerId)
    {
        var tournamentGuid = new Guid(tournamentId);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentGuid);

        CurrentPlayer = context.GetCurrentPlayer(currentPlayerId);

        var currentUserId = userManager.GetUserId(User);
        IsOwner = Tournament.OwnerId == currentUserId;
    }
}
