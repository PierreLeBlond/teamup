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

public class EditTournamentInput
{
    [Required(ErrorMessage = "provide a name between 3 and 60 characters")]
    [StringLength(
        60,
        MinimumLength = 3,
        ErrorMessage = "provide a name between 3 and 60 characters"
    )]
    public required string Name { get; set; }
}

public class EditTournamentModel(ApplicationDbContext context, UserManager<User> userManager)
    : TournamentPageModel(context, userManager)
{
    [TempData]
    public string FormResult { get; set; } = "";

    [BindProperty]
    public EditTournamentInput Input { get; set; } = null!;

    public IActionResult OnGet(string tournamentId, string? currentPlayerId)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, currentPlayerId);
        Input = new EditTournamentInput { Name = Tournament.Name };

        if (!IsOwner)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string tournamentId, string? currentPlayerId)
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

        Tournament.Name = Input.Name;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("Input.Name", $"tournament's name already exists");
            return Page();
        }

        FormResult = "tournament renamed";

        return RedirectToTournaments();
    }
}
