using System.Net.Http.Headers;
using System.Web;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Webapp.Data;
using Webapp.Models;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class EditTeamFixture<TProgram> : CustomWebApplicationFactory<TProgram>
    where TProgram : class
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public static readonly Guid EditGameId = new("543f6a09-90af-4fba-9b4b-9e86fe0b6b62");
    public static readonly Guid EditWithoutResultTeamId =
        new("543f6a09-90af-4fba-9b4b-9e863e0b6b63");
    public static readonly Guid EditTeamId = new("543f6a09-90af-4fba-9b4b-9e86fe0b6b63");

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

                    context.Games.Add(
                        new Game
                        {
                            Id = EditGameId,
                            Name = "team game",
                            TournamentId = TournamentId,
                            ShouldMaximizeScore = true,
                            NumberOfTeams = 2
                        }
                    );

                    context.Rewards.AddRange(
                        [
                            new Reward { GameId = EditGameId, Value = 100 },
                            new Reward { GameId = EditGameId, Value = 50 }
                        ]
                    );

                    context.Teams.Add(
                        new Team
                        {
                            Id = EditWithoutResultTeamId,
                            GameId = EditGameId,
                            Number = 1
                        }
                    );

                    context.Teams.Add(
                        new Team
                        {
                            Id = EditTeamId,
                            GameId = EditGameId,
                            Number = 2,
                            Bonus = 100,
                            Malus = 200
                        }
                    );
                    context.Results.Add(new Result { TeamId = EditTeamId, Value = 3000 });
                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        });
    }
}

public class EditTeamTests(EditTeamFixture<Program> factory)
    : IClassFixture<EditTeamFixture<Program>>
{
    private readonly EditTeamFixture<Program> factory = factory;

    private static readonly string path =
        $"/tournaments/{EditTeamFixture<Program>.TournamentId}/games/{EditTeamFixture<Program>.EditGameId}/teams/{EditTeamFixture<Program>.EditTeamId}/edit";
    private static readonly string pathWithoutResult =
        $"/tournaments/{EditTeamFixture<Program>.TournamentId}/games/{EditTeamFixture<Program>.EditGameId}/teams/{EditTeamFixture<Program>.EditWithoutResultTeamId}/edit";

    public async Task<HttpResponseMessage> GetResponse(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    public async Task<HttpResponseMessage> PostResponse(
        HttpClient client,
        string path,
        string bonus,
        string malus,
        string result
    )
    {
        var response = await GetResponse(client, path);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var form =
            (IHtmlFormElement?)content.QuerySelector("form")
            ?? throw new Exception("form not found");
        return await client.SendAsync(
            form,
            new Dictionary<string, string>
            {
                ["Input.Bonus"] = bonus,
                ["Input.Malus"] = malus,
                ["Input.ResultValue"] = result
            }
        );
    }

    [Fact]
    public async Task Get_ShowForm()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client, path);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Edit team");
        var bonusInput = HtmlHelpers.FindInputByLabel(content, "bonus");
        var malusInput = HtmlHelpers.FindInputByLabel(content, "malus");
        var resultInput = HtmlHelpers.FindInputByLabel(content, "result");
        var button = content.QuerySelector("button");

        Assert.NotNull(title);
        Assert.NotNull(bonusInput);
        Assert.NotNull(malusInput);
        Assert.NotNull(resultInput);
        Assert.NotNull(button);
        Assert.Equal("Edit", button.TextContent);
    }

    [Fact]
    public async Task Post_Submit_EditWithoutExistingResult()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");
        var response = await PostResponse(client, pathWithoutResult, "100", "200", "4000");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        var responsePath =
            $"/tournaments/{EditTeamFixture<Program>.TournamentId}/games/{EditTeamFixture<Program>.EditGameId}/teams/{EditTeamFixture<Program>.EditWithoutResultTeamId}";
        Assert.Equal(responsePath, HttpUtility.UrlDecode(responseContent.BaseUrl?.PathName));

        var feedback = HtmlHelpers.FindElementByText(responseContent, "team edited");

        Assert.NotNull(feedback);
    }

    [Fact]
    public async Task Post_Submit_EditAndRedirectWithFeedback()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");
        var response = await PostResponse(client, path, "100", "200", "4000");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        var responsePath =
            $"/tournaments/{EditTeamFixture<Program>.TournamentId}/games/{EditTeamFixture<Program>.EditGameId}/teams/{EditTeamFixture<Program>.EditTeamId}";
        Assert.Equal(responsePath, HttpUtility.UrlDecode(responseContent.BaseUrl?.PathName));

        var feedback = HtmlHelpers.FindElementByText(responseContent, "team edited");

        Assert.NotNull(feedback);
    }
}
