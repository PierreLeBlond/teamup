using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages;

public class TournamentModel(ApplicationDbContext context, UserManager<User> userManager)
    : TournamentPageModel(context, userManager)
{
    [TempData]
    public string FormResult { get; set; } = "";

    public List<Player> Players { get; set; } = [];
    public IList<Game> Games { get; set; } = [];

    protected override void SetModel(string tournamentId, string? currentPlayerId)
    {
        base.SetModel(tournamentId, currentPlayerId);

        Players = [.. context.Players.Where(p => p.TournamentId == Tournament.Id)];

        foreach (var player in Players)
        {
            player.Score = context.GetPlayerScore(Tournament, player.Id);
        }

        Players.Sort(
            (p1, p2) =>
            {
                var compare = p1.Score.CompareTo(p2.Score);
                if (compare != 0)
                {
                    return compare;
                }
                return p1.Name.CompareTo(p2.Name);
            }
        );

        Games = [.. context.Games.Where(g => g.TournamentId == Tournament.Id).OrderBy(g => g.Name)];
    }

    public void OnGet(string tournamentId, string? currentPlayerId)
    {
        SetModel(tournamentId, currentPlayerId);
    }
}
