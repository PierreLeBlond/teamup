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
    [Required(ErrorMessage = "provide a name between 3 and 60 characters")]
    [StringLength(
        60,
        MinimumLength = 3,
        ErrorMessage = "provide a name between 3 and 60 characters"
    )]
    [Display(Name = "name")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Thou must provide a number of teams between 1 and 300.")]
    [Range(1, 300, ErrorMessage = "Thou must provide a number of teams between 1 and 300.")]
    [Display(Name = "number of teams")]
    public required int NumberOfTeams { get; set; }

    [Display(Name = "should maximize score")]
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
            Tournament = Tournament,
            NumberOfTeams = Input.NumberOfTeams,
            ShouldMaximizeScore = Input.ShouldMaximizeScore
        };

        context.Games.Add(game);

        for (var i = 0; i < Input.NumberOfTeams; i++)
        {
            var reward = new Reward { Value = 0, Game = game };
            context.Rewards.Add(reward);
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("Input.Name", "game's name already exists");
            return Page();
        }

        FormResult = "game created";

        return RedirectToTournaments();
    }
}
