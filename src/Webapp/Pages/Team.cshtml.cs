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
    public Team Team { get; set; } = null!;
    public IList<Teammate> Teammates { get; set; } = [];
    public bool IsOwner { get; set; } = false;

    private void SetModel(string tournament, string game, string team)
    {
        var tournamentId = new Guid(tournament);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentId);
        var gameId = new Guid(game);
        Game = context.Games.Single(g => g.Id == gameId);
        var teamId = new Guid(team);
        Team = context.Teams.Single(t => t.Id == teamId);

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

    public IActionResult OnGet(string tournament, string game, string team)
    {
        SetModel(tournament, game, team);
        return Page();
    }
}
