using System.Net.Http.Headers;
using AngleSharp.Html.Dom;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Webapp.Tests.Helpers;
using Xunit.Abstractions;

namespace Webapp.Tests.Pages;

public class TournamentTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;
    private readonly ITestOutputHelper output = output;

    private static readonly string path = "/Tournaments/JaneTournament/";

    private static async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    public static async Task<HttpResponseMessage> PostResponse(HttpClient client, string playerName)
    {
        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var form =
            (IHtmlFormElement?)content.QuerySelector("form")
            ?? throw new Exception("form not found");
        return await client.SendAsync(
            form,
            new Dictionary<string, string> { ["Input.Name"] = playerName }
        );
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowTournament()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "JaneTournament");

        Assert.NotNull(title);
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowPlayers()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Behold the mighty players!");
        var player1 = HtmlHelpers.FindElementByText(content, "player1");
        var player2 = HtmlHelpers.FindElementByText(content, "player2");

        Assert.NotNull(title);
        Assert.NotNull(player1);
        Assert.NotNull(player2);
    }

    [Fact]
    public async Task Get_NotOwner_HidePlayerForm()
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var form = content.QuerySelector("form");

        Assert.Null(form);
    }

    [Fact]
    public async Task Get_Owner_ShowPlayerForm()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Add one, if thou darest!");
        var label = content.QuerySelector("label");
        var input = content.QuerySelector("input");
        var button = content.QuerySelector("button");

        Assert.NotNull(title);
        Assert.NotNull(label);
        Assert.Contains("Name", label.TextContent);
        Assert.NotNull(input);
        Assert.NotNull(button);
        Assert.Contains("Add", button.TextContent);
    }

    [Fact]
    public async Task Post_SubmitWithEmptyName_ShowError()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, "");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        // A ModelState failure returns to Page (200-OK) and doesn't redirect.
        response.EnsureSuccessStatusCode();
        Assert.Null(response.Headers.Location?.OriginalString);
        Assert.NotNull(
            HtmlHelpers.FindElementByText(
                responseContent,
                "Thou must provide a name between 3 and 60 characters."
            )
        );
    }

    [Fact]
    public async Task Post_SubmitWithAlreadyExistingName_ShowError()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, "player1");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(
            HtmlHelpers.FindElementByText(
                responseContent,
                "A player by the name of 'player1' doth already exist."
            )
        );
    }

    [Fact]
    public async Task Post_SubmitWithValidName_CreateAndRedirectWithFeedback()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, "ValidPlayer");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(HtmlHelpers.FindElementByText(responseContent, "ValidPlayer"));
        Assert.NotNull(
            HtmlHelpers.FindElementByText(
                responseContent,
                "A player named 'ValidPlayer' hath been created."
            )
        );
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowGames()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "And the games they ought to play!");
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
        Assert.EndsWith("/Tournaments/JaneTournament/CreateGame", link.Href);
    }
}
