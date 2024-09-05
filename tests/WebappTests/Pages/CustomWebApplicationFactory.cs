using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Tests.Pages;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public static readonly Guid TeamId = new("543f6a09-90af-4fba-9b4b-9e86fe0b6b6d");
    public static readonly Guid GameId = new("543f6a09-90af-4fba-9b4b-9e86fe0b6b6f");
    public static readonly Guid TournamentId = new("543f6a09-90af-4fba-9b4b-9e86fe0b6b72");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var serviceProvider = services.BuildServiceProvider();
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var context =
                            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        var tournament = new Tournament
                        {
                            Id = TournamentId,
                            Name = "jane tournament",
                            OwnerId = "JaneId"
                        };
                        context.Tournaments.Add(tournament);

                        context.Tournaments.Add(
                            new Tournament { Name = "john tournament", OwnerId = "JohnId" }
                        );

                        context.Players.Add(
                            new Player { Name = "player1", TournamentId = tournament.Id }
                        );
                        context.Players.Add(
                            new Player { Name = "player2", TournamentId = tournament.Id }
                        );

                        var game = new Game
                        {
                            Id = GameId,
                            Name = "game",
                            TournamentId = tournament.Id,
                            NumberOfTeams = 2,
                            ShouldMaximizeScore = true
                        };
                        context.Games.Add(game);
                        context.Rewards.Add(new Reward { GameId = game.Id, Value = 100 });
                        context.Rewards.Add(new Reward { GameId = game.Id, Value = 50 });

                        var team1 = new Team
                        {
                            Id = TeamId,
                            GameId = game.Id,
                            Number = 1,
                            Bonus = 100,
                            Malus = 200
                        };
                        context.Teams.Add(team1);
                        var result1 = new Result { TeamId = team1.Id, Value = 3000 };
                        context.Results.Add(result1);
                        context.Teams.Add(new Team { GameId = game.Id, Number = 2 });

                        context.SaveChanges();
                    }
                    _databaseInitialized = true;
                }
            }
        });

        builder.UseEnvironment("Development");
    }
}
