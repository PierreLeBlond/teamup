using System.Net.Http.Headers;
using System.Web;
using AngleSharp.Html.Dom;
using Webapp.Tests.Helpers;
using Xunit.Abstractions;

namespace Webapp.Tests.Pages;

public class GamesTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;
    private readonly ITestOutputHelper output = output;

    private static async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var path =
            $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}/games/{CustomWebApplicationFactory<Program>.GameId}";
        var response = await client.GetAsync(path);
        return response;
    }

    public static async Task<HttpResponseMessage> PostResponse(
        HttpClient client
    )
    {
        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var form =
            (IHtmlFormElement?)content.QuerySelector("form")
            ?? throw new Exception("form not found");
        return await client.SendAsync(
            form,
            []
        );
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowGame()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "game");

        Assert.NotNull(title);
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowRewards()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Rewards");
        var reward1 = HtmlHelpers.FindElementByText(content, "100");
        var reward2 = HtmlHelpers.FindElementByText(content, "50");

        Assert.NotNull(title);
        Assert.NotNull(reward1);
        Assert.NotNull(reward2);
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowMessageIfTeamAreNotCreated()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var message = HtmlHelpers.FindElementByText(content, "Teams have not been formed yet.");

        Assert.NotNull(message);
    }

    [Fact]
    public async Task Get_Owner_ShowGenerateTeamsButton()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var button = HtmlHelpers.FindElementByText(content, "Generate teams");

        Assert.NotNull(button);
    }

    [Fact]
    public async Task Post_Owner_GenerateTeams() {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var path =
            $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}/games/{CustomWebApplicationFactory<Program>.GameId}";
        Assert.EndsWith(path, HttpUtility.UrlDecode(content.BaseUrl?.PathName));

        var team1 = HtmlHelpers.FindElementByText(content, "Team 1");
        var team2 = HtmlHelpers.FindElementByText(content, "Team 2");
        var feedback = HtmlHelpers.FindElementByText(content, "Teams for game 'game' have been generated !");

        Assert.NotNull(team1);
        Assert.NotNull(team2);
        Assert.NotNull(feedback);
     }
}
