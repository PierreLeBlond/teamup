using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class UpdateRewardsModel(
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
    public Game Game { get; set; } = null!;

    [BindProperty]
    public IList<Reward> Rewards { get; set; } = [];

    private void SetModel(string tournament, string game)
    {
        Tournament = context.Tournaments.Single(t => t.Name == tournament);
        var currentUserId = userManager.GetUserId(User);

        var gameId = new Guid(game);
        Game = context.Games.Single(g => g.Id == gameId && g.TournamentId == tournament);
    }

    public async Task<IActionResult> OnGet(string tournament, string game)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournament, game);
        var gameId = new Guid(game);
        Rewards = [.. context.Rewards.Where(r => r.GameId == gameId).OrderBy(r => r.Value)];

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

    public async Task<IActionResult> OnPost(string tournament, string game)
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

        foreach (var reward in Rewards)
        {
            context.Rewards.Update(reward);
        }

        await context.SaveChangesAsync();

        FormResult = $"Some rewards hath been updated.";

        return RedirectToPage();
    }
}
