using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class IndexModel(ApplicationDbContext context, UserManager<User> userManager) : PageModel
{
    private readonly ApplicationDbContext context = context;
    private readonly UserManager<User> userManager = userManager;

    [TempData]
    public string FormResult { get; set; } = "";

    public IList<Tournament> Tournaments { get; set; } = [];

    public void OnGet()
    {
        var currentUserId = userManager.GetUserId(User);

        Tournaments = context.GetTournaments(currentUserId);
    }
}
