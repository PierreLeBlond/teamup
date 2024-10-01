using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;
using Webapp.Utils;

namespace Webapp.Pages;

public class GameModel(ApplicationDbContext context, UserManager<User> userManager)
    : GamePageModel(context, userManager)
{
    [TempData]
    public string FormResult { get; set; } = "";

    public IList<Reward> Rewards { get; set; } = [];
    public IList<Team> Teams { get; set; } = [];

    protected override void SetModel(string tournamentId, string gameId, string? currentPlayerId)
    {
        base.SetModel(tournamentId, gameId, currentPlayerId);
        Rewards = Game.Rewards.ToList();
        Teams = Game.Teams.ToList();
    }

    public IActionResult OnGet(string tournamentId, string gameId, string? currentPlayerId)
    {
        SetModel(tournamentId, gameId, currentPlayerId);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string tournamentId,
        string gameId,
        string? currentPlayerId
    )
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, gameId, currentPlayerId);

        if (!IsOwner)
        {
            return Forbid();
        }

        context.Teams.RemoveRange(Teams);

        var teamsPlayers = TeamGenerator.GenerateTeams([.. Tournament.Players], Game.NumberOfTeams);

        for (var i = 0; i < teamsPlayers.Count; i++)
        {
            var team = new Team { Game = Game, Number = i + 1 };
            context.Teams.Add(team);
            foreach (var teamPlayer in teamsPlayers.ElementAt(i))
            {
                context.Teammates.Add(new Teammate { Team = team, Player = teamPlayer });
            }
        }

        await context.SaveChangesAsync();

        FormResult = $"teams generated";

        return RedirectToPage();
    }
}
