using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class EditRewardsInput
{
    public int Value { get; set; }
}

public class EditRewardsModel(
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
    public EditRewardsInput[] Input { get; set; } = [];

    public IQueryable<Reward> Rewards { get; set; } = null!;

    private void SetModel(string tournament, string game)
    {
        var tournamentId = new Guid(tournament);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentId);

        var currentUserId = userManager.GetUserId(User);

        var gameId = new Guid(game);
        Game = context.Games.Single(g => g.Id == gameId && g.TournamentId == tournamentId);

        Rewards = context.Rewards.Where(r => r.GameId == gameId).OrderByDescending(r => r.Value);
    }

    public async Task<IActionResult> OnGetAsync(string tournament, string game)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournament, game);
        Input = [.. Rewards.Select(r => new EditRewardsInput { Value = r.Value })];

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

        for (int i = 0; i < Input.Length; i++)
        {
            Rewards.ElementAt(i).Value = Input[i].Value;
        }

        var count = await context.SaveChangesAsync();

        FormResult = $"A total of {count} reward(s) hath been edited.";

        return RedirectToPage();
    }
}
