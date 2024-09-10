using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Webapp.Models;

namespace Webapp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User>(options)
{
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Reward> Rewards => Set<Reward>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<Teammate> Teammates => Set<Teammate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>().HasIndex(p => new { p.Name, p.TournamentId }).IsUnique();
        modelBuilder.Entity<Game>().HasIndex(g => new { g.Name, g.TournamentId }).IsUnique();

        modelBuilder.Entity<Tournament>().HasIndex(t => t.Name).IsUnique();

        // Kept for reference, I couldn't find a way to ensure a teamate's team and player are in the same tournament
        // This solution does not work as subqueries aren't allowed within check constraints
        /*modelBuilder
            .Entity<Teammate>()
            .ToTable(b =>
                b.HasCheckConstraint(
                    "CK_Teammate_TeamAndPlayerSameTournament",
                    "[TeamId] IN (SELECT TeamId FROM Teams JOIN Games ON Games.Id = GameId WHERE TournamentId = (SELECT TournamentId FROM Players WHERE Id = [PlayerId]))"
                )
            );*/

        base.OnModelCreating(modelBuilder);
    }

    public Player? GetCurrentPlayer(string? currentPlayerId)
    {
        Guid? currentPlayerGuid = currentPlayerId is null ? null : new Guid(currentPlayerId);
        var currentPlayer = currentPlayerGuid is null
            ? null
            : Players.Single(p => p.Id == currentPlayerGuid);
        return currentPlayer;
    }

    private int GetPlayerScoreFromGame(Game game, Guid playerId)
    {
        int score = 0;

        var teammate = Teammates
            .Include(t => t.Team)
            .ThenInclude(t => t.Result)
            .SingleOrDefault(t => t.Team.GameId == game.Id && t.PlayerId == playerId);
        if (teammate == null)
        {
            return score;
        }

        score += teammate.Bonus;
        score -= teammate.Malus;

        score += GetTeamScore(game, teammate.Team);

        return score;
    }

    public int GetPlayerScore(Tournament tournament, Guid playerId)
    {
        int score = 0;

        var games = Games.Where(g => g.TournamentId == tournament.Id).ToArray();

        foreach (var game in games)
        {
            score += GetPlayerScoreFromGame(game, playerId);
        }

        return score;
    }

    public int GetTeamScore(Game game, Team team)
    {
        int score = 0;

        score += team.Bonus;
        score -= team.Malus;

        if (team.Result == null)
        {
            return score;
        }

        var teams = Teams
            .Include(t => t.Result)
            .Where(t => t.GameId == game.Id && t.Result != null);
        var sortedTeams = game.ShouldMaximizeScore
            ? teams.OrderByDescending(t => t.Result!.Value)
            : teams.OrderBy(t => t.Result!.Value);

        var index = sortedTeams.ToList().IndexOf(team);
        var rewards = Rewards
            .Where(r => r.GameId == game.Id)
            .OrderByDescending(r => r.Value)
            .ToList();

        if (index < 0 || index >= rewards.Count)
        {
            return score;
        }

        score += rewards[index].Value;

        return score;
    }
}
