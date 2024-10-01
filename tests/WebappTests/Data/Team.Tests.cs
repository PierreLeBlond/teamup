using System.Globalization;
using Webapp.Models;

namespace Webapp.Tests.Data;

public class TeamTest(TestDatabaseFixture fixture) : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture Fixture { get; } = fixture;

    [Fact]
    public void ShouldCreateTeam()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();

        var tournament = new Tournament { Name = "tournamentName", OwnerName = "ownerName" };
        context.Tournaments.Add(tournament);
        var game = new Game
        {
            Name = "gameName",
            Tournament = tournament,
            ShouldMaximizeScore = true,
            NumberOfTeams = 2
        };
        context.Games.Add(game);
        var team = new Team { Game = game, Number = 1 };
        context.Teams.Add(team);
        context.SaveChanges();

        context.ChangeTracker.Clear();

        var savedTeam = context.Teams.Single(t => t.Id == team.Id);
        Assert.Equal(1, savedTeam.Number);
    }
}
