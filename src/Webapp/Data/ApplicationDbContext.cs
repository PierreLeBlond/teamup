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

    private static void Seed(ModelBuilder modelBuilder)
    {
        var tournament = new Tournament
        {
            Id = 1,
            Name = "The Great Olympiad",
            OwnerName = "pierre.lespingal@gmail.com"
        };
        modelBuilder.Entity<Tournament>().HasData(tournament);

        var player1 = new Player
        {
            Id = 1,
            Name = "Achilles",
            TournamentId = tournament.Id
        };
        var player2 = new Player
        {
            Id = 2,
            Name = "Antigone",
            TournamentId = tournament.Id
        };
        var player3 = new Player
        {
            Id = 3,
            Name = "Bellerophon",
            TournamentId = tournament.Id
        };
        var player4 = new Player
        {
            Id = 4,
            Name = "Nausica",
            TournamentId = tournament.Id
        };
        modelBuilder.Entity<Player>().HasData(player1, player2, player3, player4);

        var game1 = new Game
        {
            Id = 1,
            Name = "Hide and Seek",
            TournamentId = tournament.Id,
            ShouldMaximizeScore = true,
            NumberOfTeams = 2
        };
        var game2 = new Game
        {
            Id = 2,
            Name = "Red lights, Green lights",
            TournamentId = tournament.Id,
            ShouldMaximizeScore = true,
            NumberOfTeams = 2
        };
        modelBuilder.Entity<Game>().HasData(game1, game2);

        modelBuilder
            .Entity<Reward>()
            .HasData(
                new Reward
                {
                    Id = 1,
                    Value = 200,
                    GameId = game1.Id
                },
                new Reward
                {
                    Id = 2,
                    Value = 100,
                    GameId = game1.Id
                },
                new Reward
                {
                    Id = 3,
                    Value = 400,
                    GameId = game2.Id
                },
                new Reward
                {
                    Id = 4,
                    Value = 200,
                    GameId = game2.Id
                }
            );

        var team1 = new Team
        {
            Id = 1,
            GameId = game1.Id,
            Number = 1,
            Malus = 50
        };
        var team2 = new Team
        {
            Id = 2,
            GameId = game1.Id,
            Number = 2
        };
        var team3 = new Team
        {
            Id = 3,
            GameId = game2.Id,
            Number = 1,
            Bonus = 100
        };
        var team4 = new Team
        {
            Id = 4,
            GameId = game2.Id,
            Number = 2
        };

        modelBuilder.Entity<Team>().HasData(team1, team2, team3, team4);

        modelBuilder
            .Entity<Result>()
            .HasData(
                new Result
                {
                    Id = 1,
                    TeamId = team1.Id,
                    Value = 24
                },
                new Result
                {
                    Id = 2,
                    TeamId = team2.Id,
                    Value = 16
                },
                new Result
                {
                    Id = 3,
                    TeamId = team3.Id,
                    Value = 3
                },
                new Result
                {
                    Id = 4,
                    TeamId = team4.Id,
                    Value = 4
                }
            );

        modelBuilder
            .Entity<Teammate>()
            .HasData(
                new Teammate
                {
                    Id = 1,
                    TeamId = team1.Id,
                    PlayerId = player1.Id,
                    Bonus = 10
                },
                new Teammate
                {
                    Id = 2,
                    TeamId = team1.Id,
                    PlayerId = player2.Id,
                    Malus = 20
                },
                new Teammate
                {
                    Id = 3,
                    TeamId = team2.Id,
                    PlayerId = player3.Id
                },
                new Teammate
                {
                    Id = 4,
                    TeamId = team2.Id,
                    PlayerId = player4.Id
                },
                new Teammate
                {
                    Id = 5,
                    TeamId = team3.Id,
                    PlayerId = player1.Id
                },
                new Teammate
                {
                    Id = 6,
                    TeamId = team4.Id,
                    PlayerId = player2.Id,
                    Bonus = 10
                },
                new Teammate
                {
                    Id = 7,
                    TeamId = team3.Id,
                    PlayerId = player3.Id
                },
                new Teammate
                {
                    Id = 8,
                    TeamId = team4.Id,
                    PlayerId = player4.Id,
                    Malus = 5
                }
            );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>().HasIndex(p => new { p.Name, p.TournamentId }).IsUnique();
        modelBuilder.Entity<Game>().HasIndex(g => new { g.Name, g.TournamentId }).IsUnique();

        modelBuilder.Entity<Tournament>().HasIndex(t => t.Name).IsUnique();

        Seed(modelBuilder);

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

    public List<Tournament> GetTournaments(string? currentUserName)
    {
        return Tournaments
            .Where(tournament => tournament.OwnerName == currentUserName)
            .OrderBy(tournament => tournament.Name.ToLower())
            .ToList();
    }

    public Tournament GetTournament(int tournamentId)
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

    public Player? GetCurrentPlayer(Tournament tournament, int? currentPlayerId)
    {
        if (currentPlayerId is null)
        {
            return null;
        }
        var currentPlayer = tournament.Players.Single(p => p.Id == currentPlayerId);

        return currentPlayer;
    }

    public Team? GetCurrentTeam(Game game, Player? CurrentPlayer)
    {
        if (CurrentPlayer is null)
        {
            return null;
        }
        return game.Teams.SingleOrDefault(t =>
            t.Teammates.Any(t => t.PlayerId == CurrentPlayer.Id)
        );
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

        int? lastResult = null;
        var rank = -1;

        for (var i = 0; i < teams.Count; i++)
        {
            var team = teams[i];
            team.Score = team.Bonus;
            team.Score -= team.Malus;

            if (team.Result?.Value != lastResult)
            {
                rank++;
            }

            if (team.Result is not null)
            {
                var rewardValue = game.Rewards.ElementAt(rank).Value;
                team.Score += rewardValue;
                team.Rank = rank + 1;
            }

            lastResult = team.Result?.Value;
        }

        return teams;
    }
}
