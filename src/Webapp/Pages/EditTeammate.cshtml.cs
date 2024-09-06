using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class EditTeammateInput
{
    public required int Bonus { get; set; } = 0;

    public required int Malus { get; set; } = 0;
}

public class EditTeammateModel(
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

    [BindProperty]
    public EditTeammateInput Input { get; set; } = null!;

    public Tournament Tournament { get; set; } = null!;
    public Game Game { get; set; } = null!;
    public Team Team { get; set; } = null!;
    public Teammate Teammate { get; set; } = null!;
    public bool IsOwner { get; set; } = false;

    private void SetModel(string tournament, string game, string team, string teammate)
    {
        var tournamentId = new Guid(tournament);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentId);
        var gameId = new Guid(game);
        Game = context.Games.Single(g => g.Id == gameId);
        var teamId = new Guid(team);
        Team = context.Teams.Single(t => t.Id == teamId);
        var teammateId = new Guid(teammate);
        Teammate = context.Teammates.Include(t => t.Player).Single(t => t.Id == teammateId);

        var currentUserId = userManager.GetUserId(User);
        IsOwner = Tournament.OwnerId == currentUserId;
    }

    public async Task<IActionResult> OnGet(
        string tournament,
        string game,
        string team,
        string teammate
    )
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournament, game, team, teammate);

        Input = new EditTeammateInput { Bonus = Team.Bonus, Malus = Team.Malus };

        var isAuthorized = await authorizationService.AuthorizeAsync(
            User,
            Tournament,
            "EditPolicy"
        );

        if (!isAuthorized.Succeeded)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPost(
        string tournament,
        string game,
        string team,
        string teammate
    )
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournament, game, team, teammate);

        var isAuthorized = await authorizationService.AuthorizeAsync(
            User,
            Tournament,
            "EditPolicy"
        );

        if (!isAuthorized.Succeeded)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Teammate.Bonus = Input.Bonus;
        Teammate.Malus = Input.Malus;

        await context.SaveChangesAsync();

        FormResult = $"teammate {Teammate.Player.Name} hath been edited.";

        return Redirect($"/tournaments/{Tournament.Id}/games/{Game.Id}/teams/{Team.Id}");
    }
}
