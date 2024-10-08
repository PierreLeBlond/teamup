using System.Net.Http.Headers;
using System.Web;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Webapp.Data;
using Webapp.Models;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class EditRewardsFixture<TProgram> : CustomWebApplicationFactory<TProgram>
    where TProgram : class
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public static readonly int EditGameId = 30;

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
                            Name = "rewards game",
                            TournamentId = TournamentId,
                            ShouldMaximizeScore = true,
                            NumberOfTeams = 2
                        }
                    );
                    context.Rewards.Add(new Reward { GameId = EditGameId, Value = 100 });
                    context.Rewards.Add(new Reward { GameId = EditGameId, Value = 50 });

                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        });
    }
}

public class EditRewardsTests(EditRewardsFixture<Program> factory)
    : IClassFixture<EditRewardsFixture<Program>>
{
    private readonly EditRewardsFixture<Program> factory = factory;

    private static readonly string path =
        $"/tournaments/{EditRewardsFixture<Program>.TournamentId}/games/{EditRewardsFixture<Program>.EditGameId}/rewards/edit";

    private static async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    public static async Task<HttpResponseMessage> PostResponse(HttpClient client, string[] values)
    {
        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var formData = new Dictionary<string, string>();
        for (var i = 0; i < values.Length; i++)
        {
            formData.Add($"Input[{i}].Value", values[i]);
        }

        var form =
            (IHtmlFormElement?)content.QuerySelector("form")
            ?? throw new Exception("form not found");
        return await client.SendAsync(form, formData);
    }

    [Fact]
    public async Task Get_ShowForm()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Edit rewards");
        var reward1Input = HtmlHelpers.FindInputByLabel(content, "reward 1");
        var reward2Input = HtmlHelpers.FindInputByLabel(content, "reward 2");
        var button = content.QuerySelector("button");

        Assert.NotNull(title);
        Assert.NotNull(reward1Input);
        Assert.NotNull(reward2Input);
        Assert.NotNull(button);
        Assert.Equal("Edit", button.TextContent);
    }

    [Fact]
    public async Task Post_ValidSubmit_UpdateAndRedirectWithFeedback()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, ["200", "100"]);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        Assert.EndsWith(
            $"/tournaments/{EditRewardsFixture<Program>.TournamentId}/games/{EditRewardsFixture<Program>.EditGameId}",
            HttpUtility.UrlDecode(content.BaseUrl?.PathName)
        );

        var feedback = HtmlHelpers.FindElementByText(content, "2 reward(s) have been edited");
        Assert.NotNull(feedback);
    }
}
