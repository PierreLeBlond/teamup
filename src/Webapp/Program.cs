using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Webapp.Authorization;
using Webapp.Data;
using Webapp.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString =
    builder.Configuration.GetConnectionString("AuthConnection")
    ?? throw new InvalidOperationException("Connection string 'AuthConnection' not found.");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString);

    options.UseOpenIddict();
});

builder
    .Services.AddOpenIddict()
    // Register the OpenIddict core components.
    .AddCore(options =>
    {
        // Configure OpenIddict to use the Entity Framework Core stores and models.
        // Note: call ReplaceDefaultEntities() to replace the default entities.
        options.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>();
    });

builder
    .Services.AddOpenIddict()
    // Register the OpenIddict client components.
    .AddClient(options =>
    {
        // Note: this sample only uses the authorization code flow,
        // but you can enable the other flows if necessary.
        options.AllowAuthorizationCodeFlow();

        // Register the signing and encryption credentials used to protect
        // sensitive data like the state tokens produced by OpenIddict.
        options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        // https://github.com/openiddict/openiddict-core/issues/864#issuecomment-783432304
        options
            .UseAspNetCore()
            .EnableRedirectionEndpointPassthrough()
            .DisableTransportSecurityRequirement();

        // Register the System.Net.Http integration.
        options.UseSystemNetHttp();

        // Register the Web providers integrations.
        //
        // Note: to mitigate mix-up attacks, it's recommended to use a unique redirection endpoint
        // URI per provider, unless all the registered providers support returning a special "iss"
        // parameter containing their URL as part of authorization responses. For more information,
        // see https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
        options
            .UseWebProviders()
            .AddGitHub(options =>
            {
                string? clientId = builder.Configuration["githubClientId"];
                string? clientSecret = builder.Configuration["githubClientSecret"];
                if (clientId is null || clientSecret is null)
                {
                    throw new InvalidOperationException("GitHub configuration not found.");
                }
                options
                    .SetClientId(clientId)
                    .SetClientSecret(clientSecret)
                    .SetRedirectUri("callback/login/github");
            });
    });

builder
    .Services.AddDefaultIdentity<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

// Authorization handlers.
builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy(
        "EditPolicy",
        policy => policy.Requirements.Add(new IsOwnerAuthorizationRequirement())
    );

builder.Services.AddScoped<IAuthorizationHandler, IsOwnerAuthorizationHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();

public partial class Program { }
