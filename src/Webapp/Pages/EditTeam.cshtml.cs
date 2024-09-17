using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class EditTeamInput
{
    [Display(Name = "bonus")]
    public required int Bonus { get; set; } = 0;

    [Display(Name = "malus")]
    public required int Malus { get; set; } = 0;

    [Display(Name = "result")]
    public int? ResultValue { get; set; }
}

public class EditTeamModel(ApplicationDbContext context, UserManager<User> userManager)
    : TeamPageModel(context, userManager)
{
    [TempData]
    public string FormResult { get; set; } = "";

    [BindProperty]
    public EditTeamInput Input { get; set; } = null!;

    public Result? Result { get; set; } = null;

    protected override void SetModel(
        string tournamentId,
        string gameId,
        string teamId,
        string? currentPlayerId
    )
    {
        base.SetModel(tournamentId, gameId, teamId, currentPlayerId);
        Result = Team.Result;
    }

    public IActionResult OnGet(
        string tournamentId,
        string gameId,
        string teamId,
        string? currentPlayerId
    )
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, gameId, teamId, currentPlayerId);

        Input = new EditTeamInput
        {
            Bonus = Team.Bonus,
            Malus = Team.Malus,
            ResultValue = Result?.Value
        };

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
        string? currentPlayerId
    )
    {
        var currentUserId = userManager.GetUserId(User);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        SetModel(tournamentId, gameId, teamId, currentPlayerId);

        if (!IsOwner)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Team.Bonus = Input.Bonus;
        Team.Malus = Input.Malus;

        var value = Input.ResultValue;
        if (value is not null)
        {
            if (Result == null)
            {
                Result = new Result { TeamId = Team.Id };
                context.Results.Add(Result);
            }

            Result.Value = (int)value;
        }

        await context.SaveChangesAsync();

        FormResult = "team edited";

        return RedirectToTeams();
    }
}
