using System.Net.Http.Headers;
using System.Web;
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
    public async Task Get_Owner_ShowOwnedTournaments()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(HtmlHelpers.FindElementByText(content, "My tournaments"));
        Assert.NotNull(HtmlHelpers.FindElementByText(content, "jane tournament"));
        Assert.Null(HtmlHelpers.FindElementByText(content, "john tournament"));
    }

    [Fact]
    public async Task Get_Authenticated_ShowCreateTournamentLink()
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByText(content, "Create tournament");

        Assert.NotNull(link);
        Assert.EndsWith("/tournaments/create", HttpUtility.UrlDecode(link.Href));
    }
}
