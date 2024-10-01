using System.Net.Http.Headers;
using System.Web;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Webapp.Data;
using Webapp.Models;
using Webapp.Tests.Helpers;
using Xunit.Abstractions;

namespace Webapp.Tests.Pages;

public class GameFixture<TProgram> : CustomWebApplicationFactory<TProgram>
    where TProgram : class
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public static readonly int NotGeneratedGameId = 70;
    public static readonly int ToGenerateGameId = 71;
    public static readonly int MinimizedGameId = 72;
    public static readonly int SameResultGameId = 73;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            var serviceProvider = services.BuildServiceProvider();
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using var scope = serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    context.Games.AddRange(
                        [
                            new Game
                            {
                                Id = NotGeneratedGameId,
                                Name = "not generated game",
                                TournamentId = TournamentId,
                                ShouldMaximizeScore = true,
                                NumberOfTeams = 2
                            },
                            new Game
                            {
                                Id = ToGenerateGameId,
                                Name = "to generate game",
                                TournamentId = TournamentId,
                                ShouldMaximizeScore = true,
                                NumberOfTeams = 2
                            },
                            new Game
                            {
                                Id = MinimizedGameId,
                                Name = "minimized game",
                                TournamentId = TournamentId,
                                ShouldMaximizeScore = false,
                                NumberOfTeams = 2
                            },
                            new Game
                            {
                                Id = SameResultGameId,
                                Name = "same result game",
                                TournamentId = TournamentId,
                                ShouldMaximizeScore = true,
                                NumberOfTeams = 3
                            }
                        ]
                    );

                    context.Rewards.AddRange(
                        [
                            new Reward
                            {
                                Id = 70,
                                GameId = NotGeneratedGameId,
                                Value = 100
                            },
                            new Reward
                            {
                                Id = 71,
                                GameId = NotGeneratedGameId,
                                Value = 50
                            },
                            new Reward
                            {
                                Id = 72,
                                GameId = ToGenerateGameId,
                                Value = 100
                            },
                            new Reward
                            {
                                Id = 73,
                                GameId = ToGenerateGameId,
                                Value = 50
                            },
                            new Reward
                            {
                                Id = 74,
                                GameId = MinimizedGameId,
                                Value = 100
                            },
                            new Reward
                            {
                                Id = 75,
                                GameId = MinimizedGameId,
                                Value = 150
                            },
                            new Reward
                            {
                                Id = 76,
                                GameId = SameResultGameId,
                                Value = 100
                            },
                            new Reward
                            {
                                Id = 77,
                                GameId = SameResultGameId,
                                Value = 50
                            },
                        ]
                    );

                    Team[] teams =
                    [
                        new Team
                        {
                            Id = 70,
                            GameId = MinimizedGameId,
                            Number = 1
                        },
                        new Team
                        {
                            Id = 71,
                            GameId = MinimizedGameId,
                            Number = 2
                        },
                        new Team
                        {
                            Id = 72,
                            GameId = SameResultGameId,
                            Number = 1
                        },
                        new Team
                        {
                            Id = 73,
                            GameId = SameResultGameId,
                            Number = 2
                        },
                        new Team
                        {
                            Id = 74,
                            GameId = SameResultGameId,
                            Number = 3
                        },
                    ];

                    context.Teams.AddRange(teams);
                    context.Results.AddRange(
                        [
                            new Result
                            {
                                Id = 70,
                                Team = teams[0],
                                Value = 3000
                            },
                            new Result
                            {
                                Id = 71,
                                Team = teams[1],
                                Value = 2000
                            },
                            new Result
                            {
                                Id = 72,
                                Team = teams[2],
                                Value = 3000
                            },
                            new Result
                            {
                                Id = 73,
                                Team = teams[3],
                                Value = 3000
                            },
                            new Result
                            {
                                Id = 74,
                                Team = teams[4],
                                Value = 1000
                            },
                        ]
                    );
                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        });
    }
}

public class GameTests(GameFixture<Program> factory, ITestOutputHelper output)
    : IClassFixture<GameFixture<Program>>
{
    private readonly GameFixture<Program> factory = factory;
    private readonly ITestOutputHelper output = output;

    private static readonly string path =
        $"/tournaments/{GameFixture<Program>.TournamentId}/games/{GameFixture<Program>.GameId}";
    private static readonly string notGeneratedPath =
        $"/tournaments/{GameFixture<Program>.TournamentId}/games/{GameFixture<Program>.NotGeneratedGameId}";
    private static readonly string toGeneratePath =
        $"/tournaments/{GameFixture<Program>.TournamentId}/games/{GameFixture<Program>.ToGenerateGameId}";
    private static readonly string minimizedPath =
        $"/tournaments/{GameFixture<Program>.TournamentId}/games/{GameFixture<Program>.MinimizedGameId}";
    private static readonly string sameResultPath =
        $"/tournaments/{GameFixture<Program>.TournamentId}/games/{GameFixture<Program>.SameResultGameId}";

    private static async Task<HttpResponseMessage> GetResponse(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    public static async Task<HttpResponseMessage> PostResponse(HttpClient client, string path)
    {
        var response = await GetResponse(client, path);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var form =
            (IHtmlFormElement?)content.QuerySelector("form")
            ?? throw new Exception("form not found");
        return await client.SendAsync(form, []);
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowGame()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client, path);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "game");

        Assert.NotNull(title);
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowRewards()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client, path);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Rewards");
        var reward1 = HtmlHelpers.FindElementByText(content, "100");
        var reward2 = HtmlHelpers.FindElementByText(content, "50");

        Assert.NotNull(title);
        Assert.NotNull(reward1);
        Assert.NotNull(reward2);
    }

    [Fact]
    public async Task Get_Unauthenticated_TeamsNotGenerated_ShowMessage()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client, notGeneratedPath);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var message = HtmlHelpers.FindElementByText(content, "Teams have not been formed yet.");

        Assert.NotNull(message);
    }

    [Fact]
    public async Task Get_Unauthenticated_TeamsGenerated_ShowTeams()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client, path);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var team1 = HtmlHelpers.FindElementByText(content, "Team 1");
        var team2 = HtmlHelpers.FindElementByText(content, "Team 2");

        Assert.NotNull(team1);
        Assert.NotNull(team2);

        var descendants = content.Descendants().ToList();
        var team1Index = descendants.IndexOf(team1);
        var team2Index = descendants.IndexOf(team2);

        Assert.True(team1Index < team2Index);
    }

    [Fact]
    public async Task Get_Unauthenticated_TeamsGenerated_MinimizeGame_ShowOrderedTeams()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client, minimizedPath);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var team1 = HtmlHelpers.FindElementByText(content, "Team 1");
        var team2 = HtmlHelpers.FindElementByText(content, "Team 2");

        Assert.NotNull(team1);
        Assert.NotNull(team2);

        var descendants = content.Descendants().ToList();
        var team1Index = descendants.IndexOf(team1);
        var team2Index = descendants.IndexOf(team2);

        Assert.True(team1Index > team2Index);
    }

    [Fact]
    public async Task Get_Unhauthenticated_TeamsGenerated_GroupTeamsWithSameResult()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client, sameResultPath);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var rank1 = HtmlHelpers.FindElementByAriaLabel(content, "rank 1");
        var team1 = HtmlHelpers.FindElementByText(content, "Team 1");
        var team2 = HtmlHelpers.FindElementByText(content, "Team 2");
        var rank2 = HtmlHelpers.FindElementByAriaLabel(content, "rank 2");
        var team3 = HtmlHelpers.FindElementByText(content, "Team 3");

        Assert.NotNull(rank1);
        Assert.NotNull(team1);
        Assert.NotNull(team2);
        Assert.NotNull(rank2);
        Assert.NotNull(team3);

        var descendants = content.Descendants().ToList();
        var rank1Index = descendants.IndexOf(rank1);
        var team1Index = descendants.IndexOf(team1);
        var team2Index = descendants.IndexOf(team2);
        var rank2Index = descendants.IndexOf(rank2);
        var team3Index = descendants.IndexOf(team3);

        Assert.True(rank1Index < team1Index);
        Assert.True(team1Index < team2Index);
        Assert.True(team2Index < rank2Index);
        Assert.True(rank2Index < team3Index);
    }

    [Fact]
    public async Task Get_Owner_TeamsNotGenerated_ShowGenerateButton()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client, notGeneratedPath);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var button = HtmlHelpers.FindElementByText(content, "Generate teams");

        Assert.NotNull(button);
    }

    [Fact]
    public async Task Get_Owner_TeamsGenerated_ShowReGenerateButton()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client, path);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var button = HtmlHelpers.FindElementByText(content, "Re-generate teams");

        Assert.NotNull(button);
    }

    [Fact]
    public async Task Post_Owner_GenerateTeams()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, toGeneratePath);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        Assert.EndsWith(toGeneratePath, HttpUtility.UrlDecode(content.BaseUrl?.PathName));

        var team1 = HtmlHelpers.FindElementByText(content, "Team 1");
        var player1 = HtmlHelpers.FindElementByText(content, "player1");
        var team2 = HtmlHelpers.FindElementByText(content, "Team 2");
        var player2 = HtmlHelpers.FindElementByText(content, "player2");
        var feedback = HtmlHelpers.FindElementByText(content, "teams generated");

        Assert.NotNull(team1);
        Assert.NotNull(player1);
        Assert.NotNull(team2);
        Assert.NotNull(player2);
        Assert.NotNull(feedback);
    }
}
