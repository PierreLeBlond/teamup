using Webapp.Models;

namespace Webapp.Tests.Data;

public class TournamentTest(TestDatabaseFixture fixture) : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture Fixture { get; } = fixture;

    [Fact]
    public void ShouldCreateTournament()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();

        context.Tournaments.Add(
            new Tournament { Name = "tournamentName", OwnerName = "ownerName" }
        );
        context.SaveChanges();

        context.ChangeTracker.Clear();

        var tournament = context.Tournaments.Single(t => t.Name == "tournamentName");
        Assert.Equal("tournamentName", tournament.Name);
    }
}
