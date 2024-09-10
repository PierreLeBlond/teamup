using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

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
        Rewards =
        [
            .. context.Rewards.Where(r => r.GameId == Game.Id).OrderByDescending(r => r.Value)
        ];
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

        foreach (var team in Teams)
        {
            team.Score = context.GetTeamScore(Game, team);
            foreach (var teammate in team.Teammates)
            {
                teammate.Player.Score = context.GetPlayerScore(Tournament, teammate.Player.Id);
            }
        }
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
