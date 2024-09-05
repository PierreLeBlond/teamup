using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class EditTeamInput
{
    public required int Bonus { get; set; } = 0;

    public required int Malus { get; set; } = 0;

    public int? ResultValue { get; set; }
}

public class EditTeamModel(
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
    public EditTeamInput Input { get; set; } = null!;

    public Tournament Tournament { get; set; } = null!;
    public Game Game { get; set; } = null!;
    public Team Team { get; set; } = null!;
    public Result? Result { get; set; } = null;
    public bool IsOwner { get; set; } = false;

    private void SetModel(string tournament, string game, string team)
    {
        var tournamentId = new Guid(tournament);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentId);
        var gameId = new Guid(game);
        Game = context.Games.Single(g => g.Id == gameId);
        var teamId = new Guid(team);
        Team = context.Teams.Single(t => t.Id == teamId);

        Result = context.Results.SingleOrDefault(r => r.TeamId == teamId);

        var currentUserId = userManager.GetUserId(User);
        IsOwner = Tournament.OwnerId == currentUserId;
    }

    public async Task<IActionResult> OnGet(string tournament, string game, string team)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournament, game, team);

        Input = new EditTeamInput
        {
            Bonus = Team.Bonus,
            Malus = Team.Malus,
            ResultValue = Result?.Value
        };

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

    public async Task<IActionResult> OnPost(string tournament, string game, string team)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournament, game, team);

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

        Team.Bonus = Input.Bonus;
        Team.Malus = Input.Malus;

        var value = Input.ResultValue;
        if (value is not null)
        {
            if (Result == null)
            {
                Result = new Result { TeamId = Team.Id };
                context.Results.Add(Result);
            }

            Result.Value = (int)value;
        }

        await context.SaveChangesAsync();

        FormResult = $"team {Team.Number} hath been edited.";

        return Redirect($"/tournaments/{Tournament.Id}/games/{Game.Id}");
    }
}
