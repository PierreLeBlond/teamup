using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class TournamentModel(ApplicationDbContext context, UserManager<User> userManager)
    : PageModel
{
    private readonly ApplicationDbContext context = context;
    private readonly UserManager<User> userManager = userManager;

    [TempData]
    public string FormResult { get; set; } = "";

    public Tournament Tournament { get; set; } = null!;

    public Player? CurrentPlayer { get; set; } = null;
    public IList<Player> Players { get; set; } = [];
    public IList<Game> Games { get; set; } = [];
    public bool IsOwner { get; set; } = false;

    private void SetModel(string tournamentId, string? currentPlayerId)
    {
        var tournamentGuid = new Guid(tournamentId);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentGuid);

        var currentUserId = userManager.GetUserId(User);
        IsOwner = Tournament.OwnerId == currentUserId;

        CurrentPlayer = context.GetCurrentPlayer(currentPlayerId);

        Players =
        [
            .. context.Players.Where(p => p.TournamentId == Tournament.Id).OrderBy(p => p.Name)
        ];

        Games = [.. context.Games.Where(g => g.TournamentId == Tournament.Id).OrderBy(g => g.Name)];
    }

    public void OnGet(string tournamentId, string? currentPlayerId)
    {
        SetModel(tournamentId, currentPlayerId);
    }
}
