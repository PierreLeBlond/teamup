using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class EditTeammateInput
{
    [Display(Name = "bonus")]
    public required int Bonus { get; set; } = 0;

    [Display(Name = "malus")]
    public required int Malus { get; set; } = 0;
}

public class EditTeammateModel(ApplicationDbContext context, UserManager<User> userManager)
    : TeamPageModel(context, userManager)
{
    [TempData]
    public string FormResult { get; set; } = "";

    [BindProperty]
    public EditTeammateInput Input { get; set; } = null!;

    public Teammate Teammate { get; set; } = null!;

    private void SetModel(
        string tournamentId,
        string gameId,
        string teamId,
        string teammateId,
        string? currentPlayerId
    )
    {
        base.SetModel(tournamentId, gameId, teamId, currentPlayerId);

        var teammateGuid = new Guid(teammateId);
        Teammate = context.Teammates.Include(t => t.Player).Single(t => t.Id == teammateGuid);
    }

    public IActionResult OnGet(
        string tournamentId,
        string gameId,
        string teamId,
        string teammateId,
        string? currentPlayerId
    )
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, gameId, teamId, teammateId, currentPlayerId);

        Input = new EditTeammateInput { Bonus = Teammate.Bonus, Malus = Teammate.Malus };

        if (!IsOwner)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPost(
        string tournamentId,
        string gameId,
        string teamId,
        string teammateId,
        string? currentPlayerId
    )
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, gameId, teamId, teammateId, currentPlayerId);

        if (!IsOwner)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Teammate.Bonus = Input.Bonus;
        Teammate.Malus = Input.Malus;

        await context.SaveChangesAsync();

        FormResult = $"teammate {Teammate.Player.Name} hath been edited.";

        return Redirect($"/tournaments/{Tournament.Id}/games/{Game.Id}/teams/{Team.Id}");
    }
}
