using System.Net.Http.Headers;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class ProtectedPagesTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;

    public static async Task<HttpResponseMessage> GetResponse(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    [Theory]
    [InlineData("/tournaments/create")]
    [InlineData("/tournaments/jane tournament/games/create")]
    [InlineData("/tournaments/jane tournament/players/create")]
    public async Task Get_Unauthenticated_Forbiden(string path)
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client, path);

        Assert.Equal(401, (int)response.StatusCode);
    }

    [Theory]
    [InlineData("/tournaments/jane tournament/games/create")]
    [InlineData("/tournaments/jane tournament/players/create")]
    public async Task Get_AuthenticatedNotOwner_Forbiden(string path)
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );

        var response = await GetResponse(client, path);

        Assert.Equal(403, (int)response.StatusCode);
    }

    [Theory]
    [InlineData("/tournaments/jane tournament/games/create")]
    [InlineData("/tournaments/jane tournament/players/create")]
    public async Task Get_Owner_Success(string path)
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client, path);

        Assert.Equal(200, (int)response.StatusCode);
    }
}
