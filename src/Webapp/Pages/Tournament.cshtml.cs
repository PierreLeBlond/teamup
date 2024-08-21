using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class TournamentModel(
    ApplicationDbContext context,
    UserManager<User> userManager,
    IAuthorizationService authorizationService
) : PageModel
{
    private readonly ApplicationDbContext context = context;
    private readonly UserManager<User> userManager = userManager;
    private readonly IAuthorizationService authorizationService = authorizationService;

    [TempData]
    public string FormResult { get; set; } = "";

    public Tournament Tournament { get; set; } = null!;
    public IList<Player> Players { get; set; } = [];
    public IList<Game> Games { get; set; } = [];
    public bool IsOwner { get; set; } = false;

    private void SetModel(string tournament)
    {
        var tournamentId = new Guid(tournament);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentId);

        var currentUserId = userManager.GetUserId(User);
        IsOwner = Tournament.OwnerId == currentUserId;

        Players =
        [
            .. context.Players.Where(p => p.TournamentId == Tournament.Id).OrderBy(p => p.Name)
        ];

        Games = [.. context.Games.Where(g => g.TournamentId == Tournament.Id).OrderBy(g => g.Name)];
    }

    public void OnGet(string tournament)
    {
        SetModel(tournament);
    }
}