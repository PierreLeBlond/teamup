using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class CreateTournamentModel(ApplicationDbContext context, UserManager<User> userManager)
    : PageModel
{
    private readonly ApplicationDbContext context = context;
    private readonly UserManager<User> userManager = userManager;

    [TempData]
    public string FormResult { get; set; } = "";

    [BindProperty]
    public Tournament Input { get; set; } = new Tournament { Name = "", OwnerId = "" };

    public IList<Tournament> Tournaments { get; set; } = [];

    public IActionResult OnGet()
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var tournament = new Tournament { Name = Input.Name, OwnerId = currentUserId };

        context.Tournaments.Add(tournament);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                "Input.Name",
                $"A tournament by the name of '{tournament.Name}' doth already exists."
            );
            return Page();
        }

        FormResult = $"A tournament named '{tournament.Name}' hath been created.";

        return Redirect($"/tournaments/{tournament.Name}");
    }
}
