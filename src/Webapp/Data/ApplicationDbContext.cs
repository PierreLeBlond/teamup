using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Polly;
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

    public List<Tournament> GetTournaments(string? currentUserId)
    {
        return Tournaments
            .Where(tournament => tournament.OwnerId == currentUserId)
            .OrderBy(tournament => tournament.Name.ToLower())
            .ToList();
    }

    public Tournament GetTournament(Guid tournamentId)
    {
        var tournament = Tournaments
            .Include(t => t.Games.OrderBy(game => game.Name.ToLower()))
            .ThenInclude(g => g.Rewards.OrderByDescending(r => r.Value))
            .Include(t => t.Players)
            .Single(tournament => tournament.Id == tournamentId);

        var players = tournament.Players.ToList();

        foreach (var player in players)
        {
            player.Score = 0;
        }

        foreach (var game in tournament.Games)
        {
            game.Teams = GetTeams(game);
            foreach (var team in game.Teams)
            {
                foreach (var teammate in team.Teammates)
                {
                    var player = players.Single(p => p.Id == teammate.PlayerId);
                    player.Score += teammate.Bonus;
                    player.Score -= teammate.Malus;
                    player.Score += team.Score;

                    teammate.Player = player;
                }
            }
        }

        players.Sort(
            (p1, p2) =>
            {
                var compare = p2.Score.CompareTo(p1.Score);
                if (compare != 0)
                {
                    return compare;
                }
                return p1.Name.CompareTo(p2.Name);
            }
        );

        tournament.Players = players;

        return tournament;
    }

    public Player? GetCurrentPlayer(Tournament tournament, string? currentPlayerId)
    {
        Guid? currentPlayerGuid = currentPlayerId is null ? null : new Guid(currentPlayerId);

        if (currentPlayerGuid is null)
        {
            return null;
        }
        var currentPlayer = tournament.Players.Single(p => p.Id == currentPlayerGuid);

        return currentPlayer;
    }

    public Team? GetCurrentTeam(Game game, Player? CurrentPlayer)
    {
        if (CurrentPlayer is null)
        {
            return null;
        }
        return game.Teams.Single(t => t.Teammates.Any(t => t.PlayerId == CurrentPlayer.Id));
    }

    public List<Team> GetTeams(Game game)
    {
        var queryableTeams = Teams
            .Include(t => t.Result)
            .Include(t => t.Teammates)
            .Where(t => t.GameId == game.Id);
        var sortedQueryableTeams = game.ShouldMaximizeScore
            ? queryableTeams
                .OrderByDescending(t => t.Result != null ? t.Result.Value : int.MinValue)
                .ThenBy(t => t.Number)
            : queryableTeams
                .OrderBy(t => t.Result != null ? t.Result.Value : int.MaxValue)
                .ThenBy(t => t.Number);

        var teams = sortedQueryableTeams.ToList();

        for (var i = 0; i < teams.Count; i++)
        {
            var team = teams[i];
            team.Score = team.Bonus;
            team.Score -= team.Malus;

            var rewardValue = game.Rewards.ElementAt(i).Value;
            team.Score += rewardValue;

            team.Rank = i + 1;
        }

        return teams;
    }
}
