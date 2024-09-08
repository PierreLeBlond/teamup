using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class GameModel(
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

    [ViewData]
    public Tournament Tournament { get; set; } = null!;

    [ViewData]
    public Game Game { get; set; } = null!;
    public IList<Reward> Rewards { get; set; } = [];
    public IList<Team> Teams { get; set; } = [];
    public bool IsOwner { get; set; } = false;

    private void SetModel(string tournamentId, string gameId)
    {
        var tournamentGuid = new Guid(tournamentId);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentGuid);
        var gameGuid = new Guid(gameId);
        Game = context.Games.Single(g => g.Id == gameGuid);

        var currentUserId = userManager.GetUserId(User);
        IsOwner = Tournament.OwnerId == currentUserId;

        Rewards = [.. context.Rewards.Where(r => r.GameId == Game.Id).OrderBy(r => r.Value)];
        var teams = context
            .Teams.Include(t => t.Result)
            .Include(t => t.Teammates)
            .ThenInclude(t => t.Player)
            .Where(t => t.GameId == Game.Id);
        if (Game.ShouldMaximizeScore)
        {
            Teams =
            [
                .. teams.OrderByDescending(t => t.Result == null ? int.MinValue : t.Result.Value)
            ];
        }
        else
        {
            Teams = [.. teams.OrderBy(t => t.Result == null ? int.MaxValue : t.Result.Value)];
        }
    }

    public IActionResult OnGet(string tournamentId, string gameId)
    {
        SetModel(tournamentId, gameId);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string tournamentId, string gameId)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, gameId);

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

        // There is an issue if using some kind of array generation with Select, where team ids became all 0's
        var teams = new Team[Game.NumberOfTeams];
        for (var i = 0; i < Game.NumberOfTeams; i++)
        {
            var team = new Team { GameId = Game.Id, Number = i + 1 };
            teams[i] = team;
        }

        context.Teams.RemoveRange(Teams);
        context.Teams.AddRange(teams);

        for (var i = 0; i < players.Count(); i++)
        {
            var player = players.ElementAt(i);
            var team = teams.ElementAt(i % Game.NumberOfTeams);
            context.Teammates.Add(new Teammate { TeamId = team.Id, PlayerId = player.Id });
        }

        await context.SaveChangesAsync();

        FormResult = $"Teams for game '{Game.Name}' have been generated !";

        return RedirectToPage();
    }
}
