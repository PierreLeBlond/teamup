using Microsoft.EntityFrameworkCore;
using Webapp.Models;

namespace Webapp.Tests.Data;

public class GameTest(TestDatabaseFixture fixture) : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture Fixture { get; } = fixture;

    [Fact]
    public void ShouldCreateGame()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();

        context.Tournaments.Add(new Tournament { Name = "tournamentName", OwnerId = "ownerId" });
        context.Games.Add(
            new Game
            {
                Name = "gameName",
                TournamentId = "tournamentName",
                ShouldMaximizeScore = true,
                NumberOfTeams = 2
            }
        );
        context.SaveChanges();

        context.ChangeTracker.Clear();

        var game = context.Games.Single(g => g.Name == "gameName");
        Assert.Equal("gameName", game.Name);
    }
}
