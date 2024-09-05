using System.Net.Http.Headers;
using System.Web;
using AngleSharp.Html.Dom;
using Webapp.Tests.Helpers;
using Xunit.Abstractions;

namespace Webapp.Tests.Pages;

public class TeamTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;
    private readonly ITestOutputHelper output = output;

    private static readonly string path =
        $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}/games/{CustomWebApplicationFactory<Program>.GameId}/teams/{CustomWebApplicationFactory<Program>.TeamId}";

    private static async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    public static async Task<HttpResponseMessage> PostResponse(HttpClient client)
    {
        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var form =
            (IHtmlFormElement?)content.QuerySelector("form")
            ?? throw new Exception("form not found");
        return await client.SendAsync(form, []);
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowGame()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Team 1");

        Assert.NotNull(title);
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowTeammates()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var teammate = HtmlHelpers.FindElementByText(content, "player1");

        Assert.NotNull(teammate);
    }
}
