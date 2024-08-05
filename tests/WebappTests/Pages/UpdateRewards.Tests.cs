using System.Net.Http.Headers;
using System.Web;
using AngleSharp.Html.Dom;
using Webapp.Tests.Helpers;

namespace Webapp.Tests.Pages;

public class UpdateRewardsTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> factory = factory;

    private async Task<HttpResponseMessage> GetResponse(HttpClient client)
    {
        var path = $"/tournaments/{factory.TournamentId}/games/{factory.GameId}/rewards/update";
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
            formData.Add($"Rewards[{i}].Value", values[i]);
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

        var title = HtmlHelpers.FindElementByText(content, "Update Rewards");
        var reward1Input = HtmlHelpers.FindInputByLabel(content, "Reward 1");
        var reward2Input = HtmlHelpers.FindInputByLabel(content, "Reward 2");
        var button = content.QuerySelector("button");

        Assert.NotNull(title);
        Assert.NotNull(reward1Input);
        Assert.NotNull(reward2Input);
        Assert.NotNull(button);
        Assert.Equal("Create", button.TextContent);
    }

    [Fact]
    public async Task Post_ValidSubmit_UpdateAndRedirectWithFeedback()
    {
        var client = HttpClientHelpers.CreateOwnerClient(factory, allowAutoRedirect: true);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Owner");

        var response = await PostResponse(client, ["200", "100"]);
        var responseContent = await HtmlHelpers.GetDocumentAsync(response);

        var path = $"/tournaments/{factory.TournamentId}/games/{factory.GameId}/rewards/update";
        Assert.EndsWith(path, HttpUtility.UrlDecode(responseContent.BaseUrl?.PathName));

        //Assert.NotNull(HtmlHelpers.FindElementByText(responseContent, "valid player"));
        Assert.NotNull(
            HtmlHelpers.FindElementByText(responseContent, "Some rewards hath been updated.")
        );
    }
}
