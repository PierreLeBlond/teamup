using Microsoft.Extensions.DependencyInjection;
using Webapp.Data;
using Webapp.Models;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class TournamentTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;

    public static readonly string path =
        $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}";

    private async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
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
        var game = HtmlHelpers.FindElementByText(content, "game");

        Assert.NotNull(title);
        Assert.NotNull(game);
    }
}
