using System.Net.Http.Headers;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class LinksTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;

    private static readonly Guid tournamentId = CustomWebApplicationFactory<Program>.TournamentId;
    private static readonly Guid gameId = CustomWebApplicationFactory<Program>.GameId;
    private static readonly Guid teamId = CustomWebApplicationFactory<Program>.TeamId;
    private static readonly Guid teammateId = CustomWebApplicationFactory<Program>.TeammateId;

    public static async Task<HttpResponseMessage> GetResponse(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    public static IEnumerable<object[]> GetUnauthenticatedLinks() =>
        [
            [
                $"/tournaments/{tournamentId}",
                "game",
                $"/tournaments/{tournamentId}/games/{gameId}",
            ],
            [
                $"/tournaments/{tournamentId}/games/{gameId}",
                "Team 1",
                $"/tournaments/{tournamentId}/games/{gameId}/teams/{teamId}",
            ],
        ];

    public static IEnumerable<object[]> GetAuthenticatedLinks() =>
        [
            ["/", "Create tournament", "/tournaments/create"],
        ];

    public static IEnumerable<object[]> GetOwnerLinks() =>
        [
            ["/", "jane tournament", $"/tournaments/{tournamentId}"],
            [
                $"/tournaments/{tournamentId}",
                "Edit tournament",
                $"/tournaments/{tournamentId}/edit",
            ],
            [
                $"/tournaments/{tournamentId}",
                "Create players",
                $"/tournaments/{tournamentId}/players/create",
            ],
            [
                $"/tournaments/{tournamentId}",
                "Create game",
                $"/tournaments/{tournamentId}/games/create",
            ],
            [
                $"/tournaments/{tournamentId}/games/{gameId}",
                "Edit game",
                $"/tournaments/{tournamentId}/games/{gameId}/edit",
            ],
            [
                $"/tournaments/{tournamentId}/games/{gameId}",
                "Edit rewards",
                $"/tournaments/{tournamentId}/games/{gameId}/rewards/edit",
            ],
            [
                $"/tournaments/{tournamentId}/games/{gameId}/teams/{teamId}",
                "Edit team",
                $"/tournaments/{tournamentId}/games/{gameId}/teams/{teamId}/edit",
            ],
            [
                $"/tournaments/{tournamentId}/games/{gameId}/teams/{teamId}",
                "Edit player1",
                $"/tournaments/{tournamentId}/games/{gameId}/teams/{teamId}/teammates/{teammateId}/edit",
            ],
        ];

    [Theory]
    [MemberData(nameof(GetUnauthenticatedLinks))]
    public async Task Get_ShowUnauthenticatedLinks(string source, string text, string target)
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory);

        var response = await GetResponse(client, source);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByTextAndHref(content, text, target);

        Assert.NotNull(link);
    }

    [Theory]
    [MemberData(nameof(GetAuthenticatedLinks))]
    public async Task Get_Unauthenticated_HideAuthenticatedLinks(
        string source,
        string text,
        string target
    )
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var response = await GetResponse(client, source);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByTextAndHref(content, text, target);

        Assert.Null(link);
    }

    [Theory]
    [MemberData(nameof(GetAuthenticatedLinks))]
    public async Task Get_Authenticated_ShowAuthenticatedLinks(
        string source,
        string text,
        string target
    )
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );

        var response = await GetResponse(client, source);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByTextAndHref(content, text, target);

        Assert.NotNull(link);
    }

    [Theory]
    [MemberData(nameof(GetOwnerLinks))]
    public async Task Get_Authenticated_HideOwnerLinks(string source, string text, string target)
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );

        var response = await GetResponse(client, source);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByTextAndHref(content, text, target);

        Assert.Null(link);
    }

    [Theory]
    [MemberData(nameof(GetOwnerLinks))]
    public async Task Get_Owner_ShowOwnerLinks(string source, string text, string target)
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client, source);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByTextAndHref(content, text, target);

        Assert.NotNull(link);
    }
}
