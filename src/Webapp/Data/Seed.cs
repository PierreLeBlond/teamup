using Microsoft.AspNetCore.Identity;
using Webapp.Models;

namespace Webapp.Data;

public static class Seed
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var db = scope.ServiceProvider.GetRequiredService<Context>();

        db.Database.EnsureCreated();

        string name = "Administrator";

        var role = await roleManager.FindByNameAsync(name);
        if (role is null)
        {
            role = new IdentityRole(name);
            await roleManager.CreateAsync(role);
        }

        if (role.NormalizedName is null)
        {
            return;
        }

        User? user = await userManager.FindByEmailAsync("pierre.lespingal@gmail.com");
        if (user is not null)
        {
            await userManager.AddToRolesAsync(user, [role.NormalizedName]);
        }
    }
}
