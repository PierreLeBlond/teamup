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
    [Required(ErrorMessage = "provide a name between 3 and 60 characters")]
    [StringLength(
        60,
        MinimumLength = 3,
        ErrorMessage = "provide a name between 3 and 60 characters"
    )]
    [Display(Name = "name")]
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

        Game.Name = Input.Name;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("Input.Name", "game's name already exists");
            return Page();
        }

        FormResult = "game renamed";

        return RedirectToGames();
    }
}
