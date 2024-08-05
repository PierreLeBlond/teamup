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
            [(Guid tournamentId, Guid gameId) => "/tournaments/create"],
            [(Guid tournamentId, Guid gameId) => $"/tournaments/{tournamentId}/games/create"],
            [
                (Guid tournamentId, Guid gameId) =>
                    $"/tournaments/{tournamentId}/games/{gameId}/rewards/update"
            ],
            [(Guid tournamentId, Guid gameId) => $"/tournaments/{tournamentId}/players/create"]
        ];
    }

    public static IEnumerable<object[]> GetForbiddenPaths()
    {
        return
        [
            [(Guid tournamentId, Guid gameId) => $"/tournaments/{tournamentId}/games/create"],
            [
                (Guid tournamentId, Guid gameId) =>
                    $"/tournaments/{tournamentId}/games/{gameId}/rewards/update"
            ],
            [(Guid tournamentId, Guid gameId) => $"/tournaments/{tournamentId}/players/create"]
        ];
    }

    [Theory]
    [MemberData(nameof(GetUnauthorizedPaths))]
    public async Task Get_Unauthenticated_Forbiden(Func<Guid, Guid, string> getPath)
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var tournamentId = factory.TournamentId;
        var gameId = factory.GameId;
        var response = await GetResponse(client, getPath(tournamentId, gameId));

        Assert.Equal(401, (int)response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(GetForbiddenPaths))]
    public async Task Get_AuthenticatedNotOwner_Forbiden(Func<Guid, Guid, string> getPath)
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );

        var tournamentId = factory.TournamentId;
        var gameId = factory.GameId;
        var response = await GetResponse(client, getPath(tournamentId, gameId));

        Assert.Equal(403, (int)response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(GetForbiddenPaths))]
    public async Task Get_Owner_Success(Func<Guid, Guid, string> getPath)
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var tournamentId = factory.TournamentId;
        var gameId = factory.GameId;
        var response = await GetResponse(client, getPath(tournamentId, gameId));

        Assert.Equal(200, (int)response.StatusCode);
    }
}
