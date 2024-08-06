using System.Net.Http.Headers;
using System.Web;
using AngleSharp.Html.Dom;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class TournamentTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;

    private async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var path = $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}";
        var response = await client.GetAsync(path);
        return response;
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowTournament()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "jane tournament");

        Assert.NotNull(title);
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowPlayers()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Players");
        var player1 = HtmlHelpers.FindElementByText(content, "player1");
        var player2 = HtmlHelpers.FindElementByText(content, "player2");

        Assert.NotNull(title);
        Assert.NotNull(player1);
        Assert.NotNull(player2);
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowGames()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Games");
        var game1 = HtmlHelpers.FindElementByText(content, "game1");
        var game2 = HtmlHelpers.FindElementByText(content, "game2");

        Assert.NotNull(title);
        Assert.NotNull(game1);
        Assert.NotNull(game2);
    }

    [Fact]
    public async Task Get_Owner_ShowCreateGameLink()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByText(content, "Create game");

        Assert.NotNull(link);
        var path = $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}/games/create";
        Assert.EndsWith(path, HttpUtility.UrlDecode(link.Href));
    }

    [Fact]
    public async Task Get_Owner_ShowCreatePlayersLink()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByText(content, "Create players");

        Assert.NotNull(link);
        var path =
            $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}/players/create";
        Assert.EndsWith(path, HttpUtility.UrlDecode(link.Href));
    }
}
