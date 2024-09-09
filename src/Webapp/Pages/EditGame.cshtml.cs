using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class EditGameInput
{
    [Required(ErrorMessage = "Thou must provide a name between 3 and 60 characters.")]
    [StringLength(
        60,
        MinimumLength = 3,
        ErrorMessage = "Thou must provide a name between 3 and 60 characters."
    )]
    public required string Name { get; set; }
}

public class EditGameModel(ApplicationDbContext context, UserManager<User> userManager)
    : GamePageModel(context, userManager)
{
    [TempData]
    public string FormResult { get; set; } = "";

    [BindProperty]
    public EditGameInput Input { get; set; } = null!;

    public IActionResult OnGet(string tournamentId, string gameId, string? currentPlayerId)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, gameId, currentPlayerId);
        Input = new EditGameInput { Name = Game.Name };

        if (!IsOwner)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPost(
        string tournamentId,
        string gameId,
        string? currentPlayerId
    )
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, gameId, currentPlayerId);

        if (!IsOwner)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var previousName = Game.Name;
        Game.Name = Input.Name;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                "Input.Name",
                $"A game by the name of '{Game.Name}' doth already exists."
            );
            return Page();
        }

        FormResult = $"The game '{previousName}' hath been renamed to '{Game.Name}'.";

        return Redirect($"/tournaments/{Tournament.Id}/games/{Game.Id}");
    }
}
