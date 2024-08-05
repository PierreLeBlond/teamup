using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

    public Guid GameId { get; private set; }

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

                        context.Tournaments.Add(
                            new Tournament { Name = "john tournament", OwnerId = "JohnId" }
                        );
                        context.Tournaments.Add(
                            new Tournament { Name = "jane tournament", OwnerId = "JaneId" }
                        );
                        context.Players.Add(
                            new Player { Name = "player1", TournamentId = "jane tournament" }
                        );
                        context.Players.Add(
                            new Player { Name = "player2", TournamentId = "jane tournament" }
                        );
                        var game1 = new Game
                        {
                            Name = "game1",
                            TournamentId = "jane tournament",
                            NumberOfTeams = 2,
                            ShouldMaximizeScore = true
                        };
                        context.Games.Add(game1);
                        context.Rewards.Add(new Reward { GameId = game1.Id, Value = 200 });
                        context.Rewards.Add(new Reward { GameId = game1.Id, Value = 100 });
                        context.Games.Add(
                            new Game
                            {
                                Name = "game2",
                                TournamentId = "jane tournament",
                                NumberOfTeams = 1,
                                ShouldMaximizeScore = false
                            }
                        );

                        context.SaveChanges();
                    }
                    _databaseInitialized = true;
                }
            }
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                GameId = context.Games.Single(g => g.Name == "game1").Id;
            }
        });

        builder.UseEnvironment("Development");
    }
}
