using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Webapp.Tests.Helpers;

public class AuthenticatedHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "JohnId"),
            new Claim(ClaimTypes.Name, "John"),
            new Claim(ClaimTypes.Email, "john.doe@gmail.com"),
        };
        var identity = new ClaimsIdentity(claims, "Authenticated");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Authenticated");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}

public class AdminHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "JaneId"),
            new Claim(ClaimTypes.Name, "Jane"),
            new Claim(ClaimTypes.Email, "jane.doe@gmail.com"),
            new Claim(ClaimTypes.Role, "Administrator"),
        };
        var identity = new ClaimsIdentity(claims, "Admin");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Admin");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
