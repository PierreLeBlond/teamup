using System.Net.Http.Headers;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class ProtectedPagesTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;
    private static readonly int tournamentId = CustomWebApplicationFactory<Program>.TournamentId;
    private static readonly int gameId = CustomWebApplicationFactory<Program>.GameId;
    private static readonly int teamId = CustomWebApplicationFactory<Program>.TeamId;

    private static async Task<HttpResponseMessage> GetResponse(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    public static IEnumerable<object[]> GetUnhautorizedPaths() =>
        [
            ["/tournaments/create"],
            [$"/tournaments/{tournamentId}/edit"],
            [$"/tournaments/{tournamentId}/games/create"],
            [$"/tournaments/{tournamentId}/games/{gameId}/edit"],
            [$"/tournaments/{tournamentId}/games/{gameId}/rewards/edit"],
            [$"/tournaments/{tournamentId}/games/{gameId}/teams/{teamId}/edit"],
            [$"/tournaments/{tournamentId}/players/create"]
        ];

    public static IEnumerable<object[]> GetForbidenPaths() =>
        [
            [$"/tournaments/{tournamentId}/edit"],
            [$"/tournaments/{tournamentId}/games/create"],
            [$"/tournaments/{tournamentId}/games/{gameId}/edit"],
            [$"/tournaments/{tournamentId}/games/{gameId}/rewards/edit"],
            [$"/tournaments/{tournamentId}/games/{gameId}/teams/{teamId}/edit"],
            [$"/tournaments/{tournamentId}/players/create"],
        ];

    [Theory]
    [MemberData(nameof(GetUnhautorizedPaths))]
    public async Task Get_Unauthenticated_Forbiden(string path)
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client, path);

        Assert.Equal(401, (int)response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(GetForbidenPaths))]
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
    [MemberData(nameof(GetForbidenPaths))]
    public async Task Get_Owner_Success(string path)
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client, path);

        Assert.Equal(200, (int)response.StatusCode);
    }
}
