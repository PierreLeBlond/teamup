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
        var game = HtmlHelpers.FindElementByText(content, "game");
        var editableGame = HtmlHelpers.FindElementByText(content, "editable game");
        var generatedGame = HtmlHelpers.FindElementByText(content, "generated game");

        Assert.NotNull(title);
        Assert.NotNull(game);
        Assert.NotNull(editableGame);
        Assert.NotNull(generatedGame);
    }
}
