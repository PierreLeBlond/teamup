using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class GameModel(ApplicationDbContext context, UserManager<User> userManager, IAuthorizationService authorizationService) : PageModel
{
    private readonly ApplicationDbContext context = context;
    private readonly UserManager<User> userManager = userManager;
    private readonly IAuthorizationService authorizationService = authorizationService;

    [TempData]
    public string FormResult { get; set; } = "";

    public Tournament Tournament { get; set; } = null!;
    public Game Game { get; set; } = null!;
    public IList<Reward> Rewards { get; set; } = [];
    public IList<Team> Teams { get; set; } = [];
    public bool IsOwner { get; set; } = false;

    private void SetModel(string tournament, string game)
    {
        var tournamentId = new Guid(tournament);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentId);
        var gameId = new Guid(game);
        Game = context.Games.Single(g => g.Id == gameId && g.TournamentId == tournamentId);

        var currentUserId = userManager.GetUserId(User);
        IsOwner = Tournament.OwnerId == currentUserId;

        Rewards = [.. context.Rewards.Where(r => r.GameId == Game.Id).OrderBy(r => r.Value)];
        Teams = [.. context.Teams.Where(t => t.GameId == Game.Id).OrderBy(t => t.Number)];
    }

    public IActionResult OnGet(string tournament, string game)
    {
        SetModel(tournament, game);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string tournament, string game)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournament, game);

        var isAuthorized = await authorizationService.AuthorizeAsync(
            User,
            Tournament,
            "EditPolicy"
        );

        if (!isAuthorized.Succeeded)
        {
            return Forbid();
        }

        var players = context.Players.Where(p => p.TournamentId == Tournament.Id);

        var teams = Enumerable.Repeat(true, Game.NumberOfTeams).Select((x, i) => new Team {
            GameId = Game.Id,
            Number = i + 1
        });

        context.Teams.RemoveRange(Teams);
        context.Teams.AddRange(teams);

        await context.SaveChangesAsync();

        FormResult = $"Teams for game '{Game.Name}' have been generated !";

        return RedirectToPage();
    }
}
