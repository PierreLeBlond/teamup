using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Webapp.Tests.Pages.Tournaments;

public class CreateTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory = factory;

    [Fact]
    public async Task Get_RedirectToLogin_WhenNotAdministrator()
    {
        var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );

        var response = await client.GetAsync("/Tournaments/Create");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith(
            "http://localhost/Identity/Account/Login",
            response.Headers.Location.OriginalString
        );
    }
}
