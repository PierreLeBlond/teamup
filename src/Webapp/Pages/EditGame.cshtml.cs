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

public class EditGameModel(
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
    public EditGameInput Input { get; set; } = null!;

    public Tournament Tournament { get; set; } = null!;
    public Game Game { get; set; } = null!;

    private void SetModel(string tournament, string game)
    {
        var tournamentId = new Guid(tournament);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentId);

        var currentUserId = userManager.GetUserId(User);

        var gameId = new Guid(game);
        Game = context.Games.Single(g => g.Id == gameId && g.TournamentId == tournamentId);
    }

    public async Task<IActionResult> OnGetAsync(string tournament, string game)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournament, game);
        Input = new EditGameInput { Name = Game.Name };

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

    public async Task<IActionResult> OnPostAsync(string tournament, string game)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournament, game);

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
