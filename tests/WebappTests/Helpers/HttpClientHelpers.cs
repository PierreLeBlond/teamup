using System.Net.Http.Headers;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Webapp.Tests.Helpers;

public static class HttpClientHelpers
{
    public static HttpClient CreateUnauthenticatedClient(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
        return client;
    }

    public static HttpClient CreateAuthenticatedClient(
        WebApplicationFactory<Program> factory,
        bool allowAutoRedirect = false
    )
    {
        var client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services
                        .AddAuthentication("Authenticated")
                        .AddScheme<AuthenticationSchemeOptions, AuthenticatedHandler>(
                            "Authenticated",
                            options => { }
                        );
                });
            })
            .CreateClient(
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = allowAutoRedirect }
            );
        return client;
    }

    public static HttpClient CreateAdminClient(
        WebApplicationFactory<Program> factory,
        bool allowAutoRedirect = false
    )
    {
        var client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services
                        .AddAuthentication("Admin")
                        .AddScheme<AuthenticationSchemeOptions, AdminHandler>(
                            "Admin",
                            options => { }
                        );
                });
            })
            .CreateClient(
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = allowAutoRedirect }
            );
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Admin");
        return client;
    }
}
