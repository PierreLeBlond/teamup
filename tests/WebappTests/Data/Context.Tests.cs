using Webapp.Models;

namespace Webapp.Tests.Data;

public class ContextTest(TestDatabaseFixture fixture) : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture Fixture { get; } = fixture;

    [Fact]
    public void ShouldCreateContext()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();

        Assert.NotNull(context);
    }

    [Fact]
    public void ShouldGetPlayerscore()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();

        var tournament = new Tournament { Name = "tournamentName", OwnerId = "ownerId" };
        context.Tournaments.Add(tournament);
        var player1 = new Player { Name = "player1", TournamentId = tournament.Id };
        var player2 = new Player { Name = "player2", TournamentId = tournament.Id };
        context.Players.Add(player1);
        context.Players.Add(player2);

        var game1 = new Game
        {
            Name = "game1",
            TournamentId = tournament.Id,
            ShouldMaximizeScore = true,
            NumberOfTeams = 2
        };
        context.Games.Add(game1);
        context.Rewards.AddRange(
            [
                new Reward { GameId = game1.Id, Value = 100 },
                new Reward { GameId = game1.Id, Value = 50 }
            ]
        );
        var team1 = new Team
        {
            GameId = game1.Id,
            Number = 1,
            Bonus = 200,
            Malus = 100
        };
        context.Teams.Add(team1);
        var result1 = new Result { TeamId = team1.Id, Value = 3000 };
        context.Results.Add(result1);
        var teammate1 = new Teammate
        {
            TeamId = team1.Id,
            PlayerId = player1.Id,
            Bonus = 450,
            Malus = 500
        };
        context.Teammates.Add(teammate1);
        var team2 = new Team { GameId = game1.Id, Number = 2 };
        context.Teams.Add(team2);
        var result2 = new Result { TeamId = team2.Id, Value = 2000 };
        context.Results.Add(result2);
        var teammate2 = new Teammate { TeamId = team2.Id, PlayerId = player2.Id };
        context.Teammates.Add(teammate2);

        context.SaveChanges();

        context.ChangeTracker.Clear();

        var score = context.GetPlayerScore(tournament, player1.Id);

        Assert.Equal(150, score);
    }
}
