using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class CreatePlayerInput
{
    [Required(ErrorMessage = "provide a name between 3 and 60 characters")]
    [StringLength(
        60,
        MinimumLength = 3,
        ErrorMessage = "provide a name between 3 and 60 characters"
    )]
    [Display(Name = "name")]
    public required string Name { get; set; }
}

public class CreatePlayersModel(ApplicationDbContext context, UserManager<User> userManager)
    : TournamentPageModel(context, userManager)
{
    [TempData]
    public string FormResult { get; set; } = "";

    [BindProperty]
    public CreatePlayerInput Input { get; set; } = null!;

    public IList<Player> Players { get; set; } = [];

    protected override void SetModel(string tournamentId, string? currentPlayerId)
    {
        base.SetModel(tournamentId, currentPlayerId);
        Players = Tournament.Players.ToList();
    }

    public IActionResult OnGet(string tournamentId, string? currentPlayerId)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, currentPlayerId);
        Input = new CreatePlayerInput { Name = "" };

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

        var player = new Player { Name = Input.Name, Tournament = Tournament };

        context.Players.Add(player);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("Input.Name", "player's name already exists");
            return Page();
        }

        FormResult = "player created";

        return RedirectToPage();
    }
}
