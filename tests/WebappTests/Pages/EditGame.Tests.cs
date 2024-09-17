using System.Net.Http.Headers;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Webapp.Data;
using Webapp.Models;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class EditGameFixture<TProgram> : CustomWebApplicationFactory<TProgram>
    where TProgram : class
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public static readonly Guid EditGameId = new("543f6a09-90af-4fba-9b4b-9e86fe0b6b66");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            var serviceProvider = services.BuildServiceProvider();
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using var scope = serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Add test data
                    context.Games.Add(
                        new Game
                        {
                            Id = EditGameId,
                            Name = "editable game",
                            TournamentId = TournamentId,
                            ShouldMaximizeScore = true,
                            NumberOfTeams = 2
                        }
                    );

                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        });
    }
}

public class EditGameTests(EditGameFixture<Program> factory)
    : IClassFixture<EditGameFixture<Program>>
{
    private readonly EditGameFixture<Program> factory = factory;

    private static readonly string path =
        $"/tournaments/{EditGameFixture<Program>.TournamentId}/games/{EditGameFixture<Program>.EditGameId}/edit";

    private static async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var response = await client.GetAsync(path);
        return response;
    }

    public static async Task<HttpResponseMessage> PostResponse(HttpClient client, string gameName)
    {
        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var form =
            (IHtmlFormElement?)content.QuerySelector("form")
            ?? throw new Exception("form not found");
        return await client.SendAsync(
            form,
            new Dictionary<string, string> { ["Input.Name"] = gameName }
        );
    }

    [Fact]
    public async Task Get_ShowForm()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Edit game");
        var nameInput = HtmlHelpers.FindInputByLabel(content, "name");
        var button = content.QuerySelector("button");

        Assert.NotNull(title);
        Assert.NotNull(nameInput);
        Assert.NotNull(button);
        Assert.Equal("Edit", button.TextContent);
    }

    [Fact]
    public async Task Post_SubmitWithEmptyName_ShowError()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, "");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        // A ModelState failure returns to Page (200-OK) and doesn't redirect.
        response.EnsureSuccessStatusCode();
        Assert.Null(response.Headers.Location?.OriginalString);
        Assert.NotNull(
            HtmlHelpers.FindElementByText(
                responseContent,
                "provide a name between 3 and 60 characters"
            )
        );
    }

    [Fact]
    public async Task Post_SubmitWithAlreadyExistingName_ShowError()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, "game");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(
            HtmlHelpers.FindElementByText(responseContent, "game's name already exists")
        );
    }

    [Fact]
    public async Task Post_SubmitWithValidName_CreateAndRedirectWithFeedback()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, "edited game");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(HtmlHelpers.FindElementByText(responseContent, "edited game"));
        Assert.NotNull(HtmlHelpers.FindElementByText(responseContent, "game renamed"));
    }
}
