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

    public static readonly int TeammateId = 10;
    public static readonly int TeamId = 10;
    public static readonly int GameId = 10;
    public static readonly int Player1Id = 10;
    public static readonly int Player2Id = 11;
    public static readonly int TournamentId = 10;

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
                            OwnerName = "Jane"
                        };
                        context.Tournaments.Add(tournament);

                        context.Tournaments.Add(
                            new Tournament { Name = "john tournament", OwnerName = "John" }
                        );

                        var player = new Player
                        {
                            Id = Player1Id,
                            Name = "player1",
                            Tournament = tournament
                        };
                        context.Players.Add(player);
                        context.Players.Add(
                            new Player
                            {
                                Id = Player2Id,
                                Name = "player2",
                                Tournament = tournament
                            }
                        );

                        var game = new Game
                        {
                            Id = GameId,
                            Name = "game",
                            Tournament = tournament,
                            NumberOfTeams = 2,
                            ShouldMaximizeScore = true
                        };
                        context.Games.Add(game);
                        context.Rewards.Add(new Reward { Game = game, Value = 100 });
                        context.Rewards.Add(new Reward { Game = game, Value = 50 });

                        var team = new Team
                        {
                            Id = TeamId,
                            Game = game,
                            Number = 1,
                            Bonus = 100,
                            Malus = 200
                        };
                        context.Teams.Add(team);
                        var result = new Result { Team = team, Value = 3000 };
                        context.Results.Add(result);
                        context.Teams.Add(new Team { Game = game, Number = 2 });

                        context.Teammates.Add(
                            new Teammate
                            {
                                Id = TeammateId,
                                Team = team,
                                Player = player,
                                Bonus = 20,
                                Malus = 10
                            }
                        );
                        context.SaveChanges();
                    }
                    _databaseInitialized = true;
                }
            }
        });

        builder.UseEnvironment("Development");
    }
}
