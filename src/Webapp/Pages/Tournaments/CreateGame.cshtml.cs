using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages.Tournaments;

public class CreateGameModel(
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

    public Tournament Tournament { get; set; } = null!;

    [BindProperty]
    public Game Input { get; set; } =
        new Game
        {
            Name = "",
            TournamentId = "",
            NumberOfTeams = 2,
            ShouldMaximizeScore = true
        };

    private void SetModel(string name)
    {
        Tournament = context.Tournaments.Single(t => t.Name == name);
    }

    public async Task<IActionResult> OnGet(string name)
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

        return Page();
    }

    public async Task<IActionResult> OnPost(string name)
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

        var game = new Game
        {
            Name = Input.Name,
            TournamentId = Tournament.Name,
            NumberOfTeams = Input.NumberOfTeams,
            ShouldMaximizeScore = Input.ShouldMaximizeScore
        };

        context.Games.Add(game);

        await context.SaveChangesAsync();

        FormResult = $"A game named '{Input.Name}' hath been created.";

        return Redirect($"/Tournaments/{Tournament.Name}");
    }
}
