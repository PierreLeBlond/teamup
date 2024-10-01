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

        var tournament = new Tournament { Name = "tournamentName", OwnerName = "ownerId" };
        context.Tournaments.Add(tournament);
        var player1 = new Player { Name = "player1", Tournament = tournament };
        var player2 = new Player { Name = "player2", Tournament = tournament };
        context.Players.Add(player1);
        context.Players.Add(player2);

        var game1 = new Game
        {
            Name = "game1",
            Tournament = tournament,
            ShouldMaximizeScore = true,
            NumberOfTeams = 2
        };
        context.Games.Add(game1);
        context.Rewards.AddRange(
            [new Reward { Game = game1, Value = 100 }, new Reward { Game = game1, Value = 50 }]
        );
        var team1 = new Team
        {
            Game = game1,
            Number = 1,
            Bonus = 200,
            Malus = 100
        };
        context.Teams.Add(team1);
        var result1 = new Result { Team = team1, Value = 3000 };
        context.Results.Add(result1);
        var teammate1 = new Teammate
        {
            Team = team1,
            Player = player1,
            Bonus = 450,
            Malus = 500
        };
        context.Teammates.Add(teammate1);
        var team2 = new Team { Game = game1, Number = 2 };
        context.Teams.Add(team2);
        var result2 = new Result { Team = team2, Value = 2000 };
        context.Results.Add(result2);
        var teammate2 = new Teammate { Team = team2, Player = player2 };
        context.Teammates.Add(teammate2);

        context.SaveChanges();

        context.ChangeTracker.Clear();

        var t = context.GetTournament(tournament.Id);
        var player = t.Players.Single(p => p.Name == "player1");

        Assert.Equal(150, player.Score);
    }
}
