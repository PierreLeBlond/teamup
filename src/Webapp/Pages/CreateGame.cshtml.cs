using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

public class CreateGameModel(ApplicationDbContext context, UserManager<User> userManager)
    : TournamentPageModel(context, userManager)
{
    [TempData]
    public string FormResult { get; set; } = "";

    [BindProperty]
    public CreateGameInput Input { get; set; } = null!;

    public IActionResult OnGet(string tournamentId, string? currentPlayerId)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, currentPlayerId);
        Input = new CreateGameInput
        {
            Name = "",
            NumberOfTeams = 2,
            ShouldMaximizeScore = true
        };

        if (!IsOwner)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPost(string tournamentId, string? currentPlayerId)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, currentPlayerId);

        if (!IsOwner)
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
