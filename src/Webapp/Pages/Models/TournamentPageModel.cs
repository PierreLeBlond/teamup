using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class TournamentPageModel(ApplicationDbContext context, UserManager<User> userManager)
    : PageModel
{
    protected readonly ApplicationDbContext context = context;
    protected readonly UserManager<User> userManager = userManager;

    public Tournament Tournament { get; set; } = null!;
    public Player? CurrentPlayer { get; set; } = null;
    public bool IsOwner { get; set; } = false;

    protected virtual void SetModel(string tournamentId, string? currentPlayerId)
    {
        Tournament = context.GetTournament(int.Parse(tournamentId));

        CurrentPlayer = context.GetCurrentPlayer(
            Tournament,
            currentPlayerId is not null ? int.Parse(currentPlayerId) : null
        );

        var currentUserName = userManager.GetUserName(User);
        IsOwner = Tournament.OwnerName == currentUserName;
    }

    protected string GetQueryString()
    {
        if (CurrentPlayer is null)
        {
            return "";
        }
        return $"?currentPlayerId={CurrentPlayer.Id}";
    }

    protected RedirectResult RedirectToTournaments()
    {
        return Redirect($"/tournaments/{Tournament.Id}{GetQueryString()}");
    }
}
