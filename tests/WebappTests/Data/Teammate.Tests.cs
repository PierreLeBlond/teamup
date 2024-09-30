using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Webapp.Models;

namespace Webapp.Tests.Data;

public class TeammateTest(TestDatabaseFixture fixture) : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture Fixture { get; } = fixture;

    [Fact]
    public void ShouldCreateTeammate()
    {
        using var context = Fixture.CreateContext();
        context.Database.BeginTransaction();

        var tournament = new Tournament { Name = "tournamentName", OwnerName = "ownerName" };
        context.Tournaments.Add(tournament);
        var player = new Player { Name = "playerName", TournamentId = tournament.Id };
        context.Players.Add(player);
        var game = new Game
        {
            Name = "gameName",
            TournamentId = tournament.Id,
            ShouldMaximizeScore = true,
            NumberOfTeams = 2
        };
        context.Games.Add(game);
        var team = new Team { GameId = game.Id, Number = 1 };
        context.Teams.Add(team);

        var teammate = new Teammate { TeamId = team.Id, PlayerId = player.Id };
        context.Teammates.Add(teammate);

        context.SaveChanges();

        context.ChangeTracker.Clear();

        var savedTeammate = context.Teammates.SingleOrDefault(t => t.Id == teammate.Id);
        Assert.NotNull(savedTeammate);
    }
}
