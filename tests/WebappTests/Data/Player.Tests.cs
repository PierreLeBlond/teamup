using Microsoft.EntityFrameworkCore;
using Webapp.Models;

namespace Webapp.Tests.Data;

public class PlayerTest(TestDatabaseFixture fixture) : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture Fixture { get; } = fixture;

    [Fact]
    public void ShouldCreatePlayer()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();

        context.Tournaments.Add(new Tournament { Name = "tournamentName", OwnerId = "ownerId" });
        context.Players.Add(new Player { Name = "playerName", TournamentId = "tournamentName" });
        context.SaveChanges();

        context.ChangeTracker.Clear();

        var player = context.Players.Single(p => p.Name == "playerName");
        Assert.Equal("playerName", player.Name);
    }

    [Fact]
    public void ShouldThrowOnIdenticPlayerWithinSameTournament()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();

        context.Tournaments.Add(new Tournament { Name = "tournamentName", OwnerId = "ownerId" });
        context.Players.Add(new Player { Name = "playerName", TournamentId = "tournamentName" });
        context.Players.Add(new Player { Name = "playerName", TournamentId = "tournamentName" });

        Assert.Throws<DbUpdateException>(() => context.SaveChanges());
    }
}
