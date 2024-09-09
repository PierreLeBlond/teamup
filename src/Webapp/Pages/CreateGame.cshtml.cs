using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class CreateGameInput
{
    [Required(ErrorMessage = "Thou must provide a name between 3 and 60 characters.")]
    [StringLength(
        60,
        MinimumLength = 3,
        ErrorMessage = "Thou must provide a name between 3 and 60 characters."
    )]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Thou must provide a number of teams between 1 and 300.")]
    [Range(1, 300, ErrorMessage = "Thou must provide a number of teams between 1 and 300.")]
    public required int NumberOfTeams { get; set; }

    public required bool ShouldMaximizeScore { get; set; } = true;
}

public class CreateGameModel(
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

    [BindProperty]
    public CreateGameInput Input { get; set; } = null!;

    private void SetModel(string tournamentId)
    {
        var tournamentGuid = new Guid(tournamentId);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentGuid);
    }

    public async Task<IActionResult> OnGet(string tournamentId)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId);
        Input = new CreateGameInput
        {
            Name = "",
            NumberOfTeams = 2,
            ShouldMaximizeScore = true
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

    public async Task<IActionResult> OnPost(string tournamentId)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId);

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

        var game = new Game
        {
            Name = Input.Name,
            TournamentId = Tournament.Id,
            NumberOfTeams = Input.NumberOfTeams,
            ShouldMaximizeScore = Input.ShouldMaximizeScore
        };

        context.Games.Add(game);

        for (var i = 0; i < Input.NumberOfTeams; i++)
        {
            var reward = new Reward { Value = 0, GameId = game.Id };
            context.Rewards.Add(reward);
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                "Input.Name",
                $"A game by the name of '{game.Name}' doth already exists."
            );
            return Page();
        }

        FormResult = $"A game named '{Input.Name}' hath been created.";

        return Redirect($"/tournaments/{Tournament.Id}");
    }
}
