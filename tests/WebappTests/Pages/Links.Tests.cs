using System.Net.Http.Headers;
using System.Web;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class LinksTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;

    public static async Task<HttpResponseMessage> GetResponse(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    public static IEnumerable<object[]> GetUnauthenticatedLinks()
    {
        return [];
    }

    public static IEnumerable<object[]> GetAuthenticatedLinks()
    {
        return
        [
            [
                (Guid tournamentId, Guid gameId) => "/",
                "Create tournament",
                (Guid tournamentId, Guid gameId) => "/tournaments/create"
            ],
        ];
    }

    public static IEnumerable<object[]> GetOwnerLinks()
    {
        return
        [
            [
                (Guid tournamentId, Guid gameId) => $"/tournaments/{tournamentId}",
                "Edit tournament",
                (Guid tournamentId, Guid gameId) => $"/tournaments/{tournamentId}/edit",
            ],
            [
                (Guid tournamentId, Guid gameId) => $"/tournaments/{tournamentId}",
                "Create players",
                (Guid tournamentId, Guid gameId) => $"/tournaments/{tournamentId}/players/create",
            ],
            [
                (Guid tournamentId, Guid gameId) => $"/tournaments/{tournamentId}",
                "Create game",
                (Guid tournamentId, Guid gameId) => $"/tournaments/{tournamentId}/games/create",
            ],
            [
                (Guid tournamentId, Guid gameId) => $"/tournaments/{tournamentId}/games/{gameId}",
                "Edit game",
                (Guid tournamentId, Guid gameId) =>
                    $"/tournaments/{tournamentId}/games/{gameId}/edit",
            ],
            [
                (Guid tournamentId, Guid gameId) => $"/tournaments/{tournamentId}/games/{gameId}",
                "Edit rewards",
                (Guid tournamentId, Guid gameId) =>
                    $"/tournaments/{tournamentId}/games/{gameId}/rewards/edit",
            ],
        ];
    }

    /*[Theory]
    [MemberData(nameof(GetUnauthenticatedLinks))]
    public async Task Get_Unauthenticated_ShowUnauthenticatedLinks(
        Func<Guid, Guid, string> getPath,
        string text,
        Func<Guid, Guid, string> getHref
    )
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var tournamentId = factory.TournamentId;
        var gameId = factory.GameId;
        var response = await GetResponse(client, getPath(tournamentId, gameId));
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByText(content, text);

        Assert.NotNull(link);
        Assert.Equal(getHref(tournamentId, gameId), HttpUtility.UrlDecode(link.Href));
    }*/

    [Theory]
    [MemberData(nameof(GetAuthenticatedLinks))]
    public async Task Get_Unauthenticated_HideAuthenticatedLinks(
        Func<Guid, Guid, string> getPath,
        string text,
        Func<Guid, Guid, string> getHref
    )
    {
        var client = HttpClientHelpers.CreateUnauthenticatedClient(factory);

        var tournamentId = CustomWebApplicationFactory<Program>.TournamentId;
        var gameId = CustomWebApplicationFactory<Program>.GameId;
        var response = await GetResponse(client, getPath(tournamentId, gameId));
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByText(content, text);

        Assert.Null(link);
    }

    [Theory]
    [MemberData(nameof(GetAuthenticatedLinks))]
    public async Task Get_Authenticated_ShowAuthenticatedLinks(
        Func<Guid, Guid, string> getPath,
        string text,
        Func<Guid, Guid, string> getHref
    )
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );

        var tournamentId = CustomWebApplicationFactory<Program>.TournamentId;
        var gameId = CustomWebApplicationFactory<Program>.GameId;
        var response = await GetResponse(client, getPath(tournamentId, gameId));
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByText(content, text);

        Assert.NotNull(link);
        Assert.EndsWith(getHref(tournamentId, gameId), HttpUtility.UrlDecode(link.Href));
    }

    [Theory]
    [MemberData(nameof(GetOwnerLinks))]
    public async Task Get_Authenticated_HideOwnerLinks(
        Func<Guid, Guid, string> getPath,
        string text,
        Func<Guid, Guid, string> getHref
    )
    {
        var client = HttpClientHelpers.CreateAuthenticatedClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Authenticated"
        );

        var tournamentId = CustomWebApplicationFactory<Program>.TournamentId;
        var gameId = CustomWebApplicationFactory<Program>.GameId;
        var response = await GetResponse(client, getPath(tournamentId, gameId));
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByText(content, text);

        Assert.Null(link);
    }

    [Theory]
    [MemberData(nameof(GetOwnerLinks))]
    public async Task Get_Owner_ShowOwnerLinks(
        Func<Guid, Guid, string> getPath,
        string text,
        Func<Guid, Guid, string> getHref
    )
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var tournamentId = CustomWebApplicationFactory<Program>.TournamentId;
        var gameId = CustomWebApplicationFactory<Program>.GameId;
        var response = await GetResponse(client, getPath(tournamentId, gameId));
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var link = HtmlHelpers.FindAnchorByText(content, text);

        Assert.NotNull(link);
        Assert.EndsWith(getHref(tournamentId, gameId), HttpUtility.UrlDecode(link.Href));
    }
}
