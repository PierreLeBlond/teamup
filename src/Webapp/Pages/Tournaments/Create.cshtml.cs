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

    [BindProperty]
    public Tournament Input { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var tournament = new Tournament { Name = Input.Name };

        context.Tournaments.Add(tournament);
        await context.SaveChangesAsync();
        return RedirectToPage("../Index");
    }
}
