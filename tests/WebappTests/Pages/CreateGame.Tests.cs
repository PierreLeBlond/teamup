using System.Net.Http.Headers;
using AngleSharp.Html.Dom;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class CreateGameTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;

    private static readonly string path = "/Tournaments/JaneTournament/CreateGame";

    public static async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    public static async Task<HttpResponseMessage> PostResponse(
        HttpClient client,
        string gameName,
        string numberOfTeams,
        string ShouldMaximizeScore
    )
    {
        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var form =
            (IHtmlFormElement?)content.QuerySelector("form")
            ?? throw new Exception("form not found");
        return await client.SendAsync(
            form,
            new Dictionary<string, string>
            {
                ["Input.Name"] = gameName,
                ["Input.NumberOfTeams"] = numberOfTeams,
                ["Input.ShouldMaximizeScore"] = ShouldMaximizeScore
            }
        );
    }

    [Fact]
    public async Task Get_Unauthenticated_Forbiden()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);

        Assert.Equal(401, (int)response.StatusCode);
    }

    [Fact]
    public async Task Get_AuthenticatedNotOwner_Forbiden()
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );

        var response = await GetResponse(client);

        Assert.Equal(403, (int)response.StatusCode);
    }

    [Fact]
    public async Task Get_Owner_ShowForm()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Create a new game");
        var nameInput = HtmlHelpers.FindInputByLabel(content, "Name");
        var numbersOfTeamsInput = HtmlHelpers.FindInputByLabel(content, "NumberOfTeams");
        var shouldMaximizeScoreInput = HtmlHelpers.FindInputByLabel(content, "ShouldMaximizeScore");
        var button = content.QuerySelector("button");

        Assert.NotNull(title);
        Assert.NotNull(nameInput);
        Assert.NotNull(numbersOfTeamsInput);
        Assert.NotNull(shouldMaximizeScoreInput);
        Assert.NotNull(button);
    }

    [Theory]
    [InlineData("ts", "0", "true")]
    [InlineData("a name way toooooooooooooooooooooooooooooooooooooooooooooo long", "301", "true")]
    public async Task Post_InvalidSubmit_ShowErrors(
        string name,
        string numberOfTeams,
        string shouldMaximizeScore
    )
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, name, numberOfTeams, shouldMaximizeScore);
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
        Assert.NotNull(
            HtmlHelpers.FindElementByText(
                responseContent,
                "Thou must provide a number of teams between 1 and 300."
            )
        );
    }

    [Fact]
    public async Task Post_Submit_CreateAndRedirectWithFeedback()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");
        var response = await PostResponse(client, "ValidGame", "2", "false");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        Assert.Equal("/Tournaments/JaneTournament", responseContent.BaseUrl?.PathName);

        Assert.NotNull(HtmlHelpers.FindElementByText(responseContent, "ValidGame"));
        Assert.NotNull(
            HtmlHelpers.FindElementByText(
                responseContent,
                "A game named 'ValidGame' hath been created."
            )
        );
    }
}