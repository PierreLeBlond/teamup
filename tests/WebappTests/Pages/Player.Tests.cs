using Microsoft.Extensions.DependencyInjection;
using Webapp.Data;
using Webapp.Models;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class PlayerTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;

    public static readonly string path1 =
        $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}?currentPlayerId={CustomWebApplicationFactory<Program>.Player1Id}";
    public static readonly string path2 =
        $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}?currentPlayerId={CustomWebApplicationFactory<Program>.Player2Id}";

    private async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var response = await client.GetAsync(path1);
        return response;
    }

    [Fact]
    public async Task Get_PlayerSelected_ShowCue()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var currentPlayer = HtmlHelpers.FindElementByTextAndAriaLabel(
            content,
            "player1",
            "current player"
        );

        Assert.NotNull(currentPlayer);
    }

    [Fact]
    public async Task Get_ShowPlayerLinks()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var player1Link = HtmlHelpers.FindAnchorByTextAndHref(content, "player1", path1);
        var player2Link = HtmlHelpers.FindAnchorByTextAndHref(content, "player2", path2);

        Assert.NotNull(player1Link);
        Assert.NotNull(player2Link);
    }
}
