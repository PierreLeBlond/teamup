using System.Net.Http.Headers;
using System.Web;
using AngleSharp.Html.Dom;
using Webapp.Tests.Helpers;
using Xunit.Abstractions;

namespace Webapp.Tests.Pages;

public class CreatePlayersTests(
    CustomWebApplicationFactory<Program> factory,
    ITestOutputHelper output
) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;
    private readonly ITestOutputHelper output = output;

    private static readonly string path =
        $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}/players/create";

    public static async Task<HttpResponseMessage> GetResponse(HttpClient client)
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
    public async Task Get_ShowForm()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Create new players");
        var nameInput = HtmlHelpers.FindInputByLabel(content, "Name");
        var button = content.QuerySelector("button");

        Assert.NotNull(title);
        Assert.NotNull(nameInput);
        Assert.NotNull(button);
        Assert.Equal("Create", button.TextContent);
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
                "A player by the name of 'player1' doth already exists."
            )
        );
    }

    [Fact]
    public async Task Post_SubmitWithValidName_CreateAndRedirectWithFeedback()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, "valid player");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        output.WriteLine("ping");
        output.WriteLine(responseContent.DocumentElement.TextContent);
        output.WriteLine("pong");

        var path =
            $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}/players/create";
        Assert.EndsWith(path, HttpUtility.UrlDecode(responseContent.BaseUrl?.PathName));

        Assert.NotNull(HtmlHelpers.FindElementByText(responseContent, "valid player"));
        Assert.NotNull(
            HtmlHelpers.FindElementByText(
                responseContent,
                "A player named 'valid player' hath been created."
            )
        );
    }
}
