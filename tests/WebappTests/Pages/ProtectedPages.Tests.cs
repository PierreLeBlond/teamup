using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

    public static IEnumerable<object[]> GetUnauthorizedPaths()
    {
        return
        [
            [(Guid gameId) => "/tournaments/create"],
            [(Guid gameId) => "/tournaments/jane tournament/games/create"],
            [(Guid gameId) => $"/tournaments/jane tournament/games/{gameId}/rewards/update"],
            [(Guid gameId) => "/tournaments/jane tournament/players/create"]
        ];
    }

    public static IEnumerable<object[]> GetForbiddenPaths()
    {
        return
        [
            [(Guid gameId) => "/tournaments/jane tournament/games/create"],
            [(Guid gameId) => $"/tournaments/jane tournament/games/{gameId}/rewards/update"],
            [(Guid gameId) => "/tournaments/jane tournament/players/create"]
        ];
    }

    [Theory]
    [MemberData(nameof(GetUnauthorizedPaths))]
    public async Task Get_Unauthenticated_Forbiden(Func<Guid, string> getPath)
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var gameId = factory.GameId;
        var response = await GetResponse(client, getPath(gameId));

        Assert.Equal(401, (int)response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(GetForbiddenPaths))]
    public async Task Get_AuthenticatedNotOwner_Forbiden(Func<Guid, string> getPath)
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );

        var gameId = factory.GameId;
        var response = await GetResponse(client, getPath(gameId));

        Assert.Equal(403, (int)response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(GetForbiddenPaths))]
    public async Task Get_Owner_Success(Func<Guid, string> getPath)
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var gameId = factory.GameId;
        var response = await GetResponse(client, getPath(gameId));

        Assert.Equal(200, (int)response.StatusCode);
    }
}
