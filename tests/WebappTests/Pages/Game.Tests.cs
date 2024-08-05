using System.Net.Http.Headers;
using System.Web;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class GameTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;

    private async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var path = $"/tournaments/jane tournament/games/{factory.GameId}";
        var response = await client.GetAsync(path);
        return response;
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowGame()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "game1");

        Assert.NotNull(title);
    }

    [Fact]
    public async Task Get_Unauthenticated_ShowRewards()
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Rewards");
        var reward1 = HtmlHelpers.FindElementByText(content, "200");
        var reward2 = HtmlHelpers.FindElementByText(content, "100");

        Assert.NotNull(title);
        Assert.NotNull(reward1);
        Assert.NotNull(reward2);
    }

    [Fact]
    public async Task Get_Owner_ShowUpdateRewardsLink()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByText(content, "Update rewards");

        Assert.NotNull(link);
        var path = $"/tournaments/jane tournament/games/{factory.GameId}/rewards/update";
        Assert.EndsWith(path, HttpUtility.UrlDecode(link.Href));
    }
}
