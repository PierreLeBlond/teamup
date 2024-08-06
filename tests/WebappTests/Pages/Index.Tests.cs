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

        var text = HtmlHelpers.FindElementByText(
            content,
            "Lo, greetings! Here beginneth the process of creating a tournament, but first thou must log in."
        );
        var link = HtmlHelpers.FindAnchorByText(content, "Create tournament");
        var title = HtmlHelpers.FindElementByText(content, "My tournaments");

        Assert.NotNull(text);
        Assert.Null(link);
        Assert.Null(title);
    }

    [Fact]
    public async Task Get_Owner_ShowOwnedTournaments()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "My tournaments");
        var janeTournament = HtmlHelpers.FindElementByText(content, "jane tournament");
        var johnTournament = HtmlHelpers.FindElementByText(content, "john tournament");

        Assert.NotNull(title);
        Assert.NotNull(janeTournament);
        Assert.Null(johnTournament);
    }
}
