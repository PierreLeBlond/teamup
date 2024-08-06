using System.Net.Http.Headers;
using System.Web;
using AngleSharp.Html.Dom;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class EditRewardsTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;

    private async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var path =
            $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}/games/{CustomWebApplicationFactory<Program>.GameId}/rewards/edit";
        var response = await client.GetAsync(path);
        return response;
    }

    public async Task<HttpResponseMessage> PostResponse(HttpClient client, string[] values)
    {
        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var formData = new Dictionary<string, string>();
        for (var i = 0; i < values.Length; i++)
        {
            formData.Add($"Input[{i}].Value", values[i]);
        }

        var form =
            (IHtmlFormElement?)content.QuerySelector("form")
            ?? throw new Exception("form not found");
        return await client.SendAsync(form, formData);
    }

    [Fact]
    public async Task Get_ShowForm()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await GetResponse(client);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var title = HtmlHelpers.FindElementByText(content, "Edit Rewards");
        var reward1Input = HtmlHelpers.FindInputByLabel(content, "Reward 1");
        var reward2Input = HtmlHelpers.FindInputByLabel(content, "Reward 2");
        var button = content.QuerySelector("button");

        Assert.NotNull(title);
        Assert.NotNull(reward1Input);
        Assert.NotNull(reward2Input);
        Assert.NotNull(button);
        Assert.Equal("Edit", button.TextContent);
    }

    [Fact]
    public async Task Post_ValidSubmit_UpdateAndRedirectWithFeedback()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, ["200", "100"]);
        var content = await HtmlHelpers.GetDocumentAsync(response);

        var path =
            $"/tournaments/{CustomWebApplicationFactory<Program>.TournamentId}/games/{CustomWebApplicationFactory<Program>.GameId}/rewards/edit";
        Assert.EndsWith(path, HttpUtility.UrlDecode(content.BaseUrl?.PathName));

        var feedback = HtmlHelpers.FindElementByText(
            content,
            "A total of 2 reward(s) hath been edited."
        );
        var reward1Input = HtmlHelpers.FindInputByLabel(content, "Reward 1");
        var reward2Input = HtmlHelpers.FindInputByLabel(content, "Reward 2");

        Assert.NotNull(feedback);
        Assert.NotNull(reward1Input);
        Assert.NotNull(reward2Input);
        Assert.Equal("200", reward1Input.Value);
        Assert.Equal("100", reward2Input.Value);
    }
}
