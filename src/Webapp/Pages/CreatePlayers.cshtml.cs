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
    [Required(ErrorMessage = "Thou must provide a name between 3 and 60 characters.")]
    [StringLength(
        60,
        MinimumLength = 3,
        ErrorMessage = "Thou must provide a name between 3 and 60 characters."
    )]
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
        Players =
        [
            .. context.Players.Where(p => p.TournamentId == Tournament.Id).OrderBy(p => p.Name)
        ];
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

        var player = new Player { Name = Input.Name, TournamentId = Tournament.Id };

        context.Players.Add(player);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                "Input.Name",
                $"A player by the name of '{player.Name}' doth already exists."
            );
            return Page();
        }

        FormResult = $"A player named '{Input.Name}' hath been created.";

        return RedirectToPage();
    }
}
