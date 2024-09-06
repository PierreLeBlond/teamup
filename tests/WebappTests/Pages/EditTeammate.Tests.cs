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

public class EditTeammateFixture<TProgram> : CustomWebApplicationFactory<TProgram>
    where TProgram : class
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public static readonly Guid EditTeammatePlayerId = new("543f4a09-90a3-4fba-9b4b-9e86fe0b6b63");
    public static readonly Guid EditTeammateId = new("543f4a09-90a3-4fba-9b4b-9e86fe0b6b63");

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

                    context.Players.Add(
                        new Player
                        {
                            Id = EditTeammatePlayerId,
                            Name = "teammate player",
                            TournamentId = TournamentId
                        }
                    );
                    context.Teammates.Add(
                        new Teammate
                        {
                            Id = EditTeammateId,
                            TeamId = TeamId,
                            PlayerId = EditTeammatePlayerId
                        }
                    );

                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        });
    }
}

public class EditTeammateTests(EditTeammateFixture<Program> factory)
    : IClassFixture<EditTeammateFixture<Program>>
{
    private readonly EditTeammateFixture<Program> factory = factory;

    private static readonly string path =
        $"/tournaments/{EditTeammateFixture<Program>.TournamentId}/games/{EditTeammateFixture<Program>.GameId}/teams/{EditTeammateFixture<Program>.TeamId}/teammates/{EditTeammateFixture<Program>.EditTeammateId}/edit";

    public async Task<HttpResponseMessage> GetResponse(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    public async Task<HttpResponseMessage> PostResponse(
        HttpClient client,
        string path,
        string bonus,
        string malus
    )
    {
        var response = await GetResponse(client, path);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var form =
            (IHtmlFormElement?)content.QuerySelector("form")
            ?? throw new Exception("form not found");
        return await client.SendAsync(
            form,
            new Dictionary<string, string> { ["Input.Bonus"] = bonus, ["Input.Malus"] = malus }
        );
    }

    [Fact]
    public async Task Get_ShowForm()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client, path);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Edit teammate teammate player");
        var bonusInput = HtmlHelpers.FindInputByLabel(content, "Bonus");
        var malusInput = HtmlHelpers.FindInputByLabel(content, "Malus");
        var button = content.QuerySelector("button");

        Assert.NotNull(title);
        Assert.NotNull(bonusInput);
        Assert.NotNull(malusInput);
        Assert.NotNull(button);
        Assert.Equal("Edit", button.TextContent);
    }

    [Fact]
    public async Task Post_Submit_EditWithFeedback()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");
        var response = await PostResponse(client, path, "100", "200");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        var responsePath =
            $"/tournaments/{EditTeamFixture<Program>.TournamentId}/games/{EditTeamFixture<Program>.GameId}/teams/{EditTeamFixture<Program>.TeamId}";
        Assert.Equal(responsePath, HttpUtility.UrlDecode(responseContent.BaseUrl?.PathName));

        var feedback = HtmlHelpers.FindElementByText(
            responseContent,
            "teammate teammate player hath been edited."
        );

        Assert.NotNull(feedback);
    }
}
