using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class TeamModel(ApplicationDbContext context, UserManager<User> userManager)
    : TeamPageModel(context, userManager)
{
    [TempData]
    public string FormResult { get; set; } = "";

    public IList<Teammate> Teammates { get; set; } = [];

    protected override void SetModel(
        string tournamentId,
        string gameId,
        string teamId,
        string? currentPlayerId
    )
    {
        base.SetModel(tournamentId, gameId, teamId, currentPlayerId);
        Teammates =
        [
            .. context
                .Teammates.Include(t => t.Player)
                .Where(t => t.TeamId == Team.Id)
                .OrderBy(t => t.Player.Name)
        ];

        foreach (var teammate in Teammates)
        {
            teammate.Player.Score = context.GetPlayerScore(Tournament, teammate.Player.Id);
        }
    }

    public IActionResult OnGet(
        string tournamentId,
        string gameId,
        string teamId,
        string? currentPlayerId
    )
    {
        SetModel(tournamentId, gameId, teamId, currentPlayerId);
        return Page();
    }
}
