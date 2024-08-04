using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages.Tournaments;

public class TournamentModel(
    ApplicationDbContext context,
    UserManager<User> userManager,
    IAuthorizationService authorizationService
) : PageModel
{
    private readonly ApplicationDbContext context = context;
    private readonly UserManager<User> userManager = userManager;
    private readonly IAuthorizationService authorizationService = authorizationService;

    [TempData]
    public string FormResult { get; set; } = "";

    [BindProperty]
    public Player Input { get; set; } = new Player { Name = "", TournamentId = "" };

    public Tournament Tournament { get; set; } = null!;

    public IList<Player> Players { get; set; } = [];
    public IList<Game> Games { get; set; } = [];

    public bool IsOwner { get; set; } = false;

    private void SetModel(string name)
    {
        Tournament = context.Tournaments.Single(t => t.Name == name);

        var currentUserId = userManager.GetUserId(User);
        IsOwner = Tournament.OwnerId == currentUserId;

        Players = context
            .Players.Where(p => p.TournamentId == Tournament.Name)
            .OrderBy(p => p.Name)
            .ToList();

        Games = context
            .Games.Where(g => g.TournamentId == Tournament.Name)
            .OrderBy(g => g.Name)
            .ToList();
    }

    public void OnGet(string name)
    {
        SetModel(name);
    }

    public async Task<IActionResult> OnPostAsync(string name)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(name);

        var isAuthorized = await authorizationService.AuthorizeAsync(
            User,
            Tournament,
            "EditPolicy"
        );

        if (!isAuthorized.Succeeded)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var player = new Player { Name = Input.Name, TournamentId = Tournament.Name };

        context.Players.Add(player);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                "Input.Name",
                $"A player by the name of '{Input.Name}' doth already exist."
            );
            return Page();
        }

        FormResult = $"A player named '{Input.Name}' hath been created.";

        Input.Name = "";
        Input.TournamentId = "";

        return RedirectToPage();
    }
}
