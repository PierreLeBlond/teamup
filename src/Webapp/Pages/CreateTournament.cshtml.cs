using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class CreateTournamentInput
{
    [Required(ErrorMessage = "Thou must provide a name between 3 and 60 characters.")]
    [StringLength(
        60,
        MinimumLength = 3,
        ErrorMessage = "Thou must provide a name between 3 and 60 characters."
    )]
    public required string Name { get; set; }
}

public class CreateTournamentModel(ApplicationDbContext context, UserManager<User> userManager)
    : PageModel
{
    private readonly ApplicationDbContext context = context;
    private readonly UserManager<User> userManager = userManager;

    [TempData]
    public string FormResult { get; set; } = "";

    [BindProperty]
    public CreateTournamentInput Input { get; set; } = null!;

    public IList<Tournament> Tournaments { get; set; } = [];

    public IActionResult OnGet()
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        Input = new CreateTournamentInput { Name = "" };

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

        return Redirect($"/tournaments/{tournament.Id}");
    }
}
