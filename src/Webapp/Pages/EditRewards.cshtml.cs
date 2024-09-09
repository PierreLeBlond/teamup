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

public class EditRewardsModel(ApplicationDbContext context, UserManager<User> userManager)
    : GamePageModel(context, userManager)
{
    [TempData]
    public string FormResult { get; set; } = "";

    [BindProperty]
    public EditRewardsInput[] Input { get; set; } = [];

    public IQueryable<Reward> Rewards { get; set; } = null!;

    protected override void SetModel(string tournamentId, string gameId, string? currentPlayerId)
    {
        base.SetModel(tournamentId, gameId, currentPlayerId);
        Rewards = context.Rewards.Where(r => r.GameId == Game.Id).OrderByDescending(r => r.Value);
    }

    public IActionResult OnGetAsync(string tournamentId, string gameId, string? currentPlayerId)
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, gameId, currentPlayerId);
        Input = [.. Rewards.Select(r => new EditRewardsInput { Value = r.Value })];

        if (!IsOwner)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string tournamentId,
        string gameId,
        string? currentPlayerId
    )
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, gameId, currentPlayerId);

        if (!IsOwner)
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
