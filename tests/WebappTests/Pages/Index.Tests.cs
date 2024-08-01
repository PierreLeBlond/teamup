using System.Net.Http.Headers;
using AngleSharp.Html.Dom;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class IndexTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;

    private static readonly string path = "/";

    public static async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    public static async Task<HttpResponseMessage> PostResponse(
        HttpClient client,
        string tournamentName
    )
    {
        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var form =
            (IHtmlFormElement?)content.QuerySelector("form")
            ?? throw new Exception("form not found");
        return await client.SendAsync(
            form,
            new Dictionary<string, string> { ["Input.Name"] = tournamentName }
        );
    }

    [Fact]
    public async Task Get_Unauthenticated_AskToLogin()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(
            HtmlHelpers.FindElementByText(
                content,
                "Lo, greetings! Here beginneth the process of creating a tournament, but first thou must log in."
            )
        );
    }

    [Fact]
    public async Task Get_Authenticated_ShowForm()
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(
            content,
            "Greetings, John. Thou mayst create a tournament, if thou wilt!"
        );
        var input = content.QuerySelector("input");
        var button = content.QuerySelector("button");

        Assert.NotNull(title);
        Assert.NotNull(input);
        Assert.NotNull(button);
    }

    [Fact]
    public async Task Get_Authenticated_ShowOwnedTournaments()
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(HtmlHelpers.FindElementByText(content, "My tournaments"));
        Assert.NotNull(HtmlHelpers.FindElementByText(content, "JohnTournament"));
    }

    [Fact]
    public async Task Post_SubmitWithEmptyName_ShowError()
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );
        var response = await PostResponse(client, "");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        // A ModelState failure returns to Page (200-OK) and doesn't redirect.
        response.EnsureSuccessStatusCode();
        Assert.Null(response.Headers.Location?.OriginalString);
        Assert.NotNull(
            HtmlHelpers.FindElementByText(responseContent, "The Name field is required.")
        );
    }

    [Fact]
    public async Task Post_SubmitWithAlreadyExistingName_ShowError()
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );
        var response = await PostResponse(client, "JohnTournament");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(
            HtmlHelpers.FindElementByText(
                responseContent,
                "A tournament named 'JohnTournament' already exists."
            )
        );
    }

    [Fact]
    public async Task Post_SubmitWithValidName_CreateAndRedirectWithFeedback()
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );
        var response = await PostResponse(client, "ValidTournament");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(HtmlHelpers.FindElementByText(responseContent, "ValidTournament"));
        Assert.NotNull(
            HtmlHelpers.FindElementByText(
                responseContent,
                "My tournament 'ValidTournament' has been created."
            )
        );
    }
}
