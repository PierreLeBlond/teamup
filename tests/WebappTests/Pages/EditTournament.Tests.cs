using System.Net.Http.Headers;
using System.Security.AccessControl;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Webapp.Data;
using Webapp.Models;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class EditTournamentFixture<TProgram> : CustomWebApplicationFactory<TProgram>
    where TProgram : class
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public static readonly int EditTournamentId = 60;

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
                    context.Tournaments.Add(
                        new Tournament
                        {
                            Id = EditTournamentId,
                            Name = "editable tournament",
                            OwnerName = "Jane"
                        }
                    );

                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        });
    }
}

public class EditTournamentTests(EditTournamentFixture<Program> factory)
    : IClassFixture<EditTournamentFixture<Program>>
{
    private readonly EditTournamentFixture<Program> factory = factory;

    private static async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var path = $"/tournaments/{EditTournamentFixture<Program>.EditTournamentId}/edit";
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
    public async Task Get_ShowForm()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Edit tournament");
        var nameInput = HtmlHelpers.FindInputByLabel(content, "Name");
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

        var response = await PostResponse(client, "jane tournament");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(
            HtmlHelpers.FindElementByText(responseContent, "tournament's name already exists")
        );
    }

    [Fact]
    public async Task Post_SubmitWithValidName_CreateAndRedirectWithFeedback()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, "edited tournament");
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        Assert.NotNull(HtmlHelpers.FindElementByText(responseContent, "edited tournament"));
        Assert.NotNull(HtmlHelpers.FindElementByText(responseContent, "tournament renamed"));
    }
}
