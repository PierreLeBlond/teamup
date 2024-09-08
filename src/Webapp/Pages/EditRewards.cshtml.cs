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

    [ViewData]
    public Tournament Tournament { get; set; } = null!;

    [ViewData]
    public Game Game { get; set; } = null!;

    [BindProperty]
    public EditRewardsInput[] Input { get; set; } = [];

    public IQueryable<Reward> Rewards { get; set; } = null!;

    private void SetModel(string tournamentId, string gameId)
    {
        var tournamentGuid = new Guid(tournamentId);
        Tournament = context.Tournaments.Single(t => t.Id == tournamentGuid);

        var currentUserId = userManager.GetUserId(User);

        var gameGuid = new Guid(gameId);
        Game = context.Games.Single(g => g.Id == gameGuid);

        Rewards = context.Rewards.Where(r => r.GameId == gameGuid).OrderByDescending(r => r.Value);
    }

    public async Task<IActionResult> OnGetAsync(string tournamentId, string gameId)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, gameId);
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

    public async Task<IActionResult> OnPostAsync(string tournamentId, string gameId)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, gameId);

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
