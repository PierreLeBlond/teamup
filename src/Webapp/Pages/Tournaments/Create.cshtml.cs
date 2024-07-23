using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages.Tournaments;

[Authorize(Roles = "Administrator")]
public class CreateModel(Context context) : PageModel
{
    private readonly Context context = context;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        var emptyTournament = new Tournament { Name = "" };

        if (
            !await TryUpdateModelAsync(
                emptyTournament,
                "tournament", // Prefix for form value.
                s => s.Name
            )
        )
        {
            return Page();
        }

        context.Tournaments.Add(emptyTournament);
        await context.SaveChangesAsync();
        return RedirectToPage("./");
    }
}
