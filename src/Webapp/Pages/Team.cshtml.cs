using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class TeamModel(
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
    public Game Game { get; set; } = null!;
    public Player? CurrentPlayer { get; set; } = null;
    public Team Team { get; set; } = null!;
    public IList<Teammate> Teammates { get; set; } = [];
    public bool IsOwner { get; set; } = false;

    private void SetModel(
        string tournamentId,
        string gameId,
        string teamId,
        string? currentPlayerId
    )
    {
        var tournamentGuid = new Guid(tournamentId);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentGuid);
        var gameGuid = new Guid(gameId);
        Game = context.Games.Single(g => g.Id == gameGuid);
        var teamGuid = new Guid(teamId);
        Team = context.Teams.Single(t => t.Id == teamGuid);

        CurrentPlayer = context.GetCurrentPlayer(currentPlayerId);

        Teammates =
        [
            .. context
                .Teammates.Include(t => t.Player)
                .Where(t => t.TeamId == Team.Id)
                .OrderBy(t => t.Player.Name)
        ];

        var currentUserId = userManager.GetUserId(User);
        IsOwner = Tournament.OwnerId == currentUserId;
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
