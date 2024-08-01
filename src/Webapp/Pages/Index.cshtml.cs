using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webapp.Authorization;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages
{
    public class IndexModel(
        ApplicationDbContext context,
        UserManager<User> userManager,
        IAuthorizationService authorizationService
    ) : PageModel
    {
        private readonly ApplicationDbContext context = context;
        private readonly UserManager<User> userManager = userManager;
        private readonly IAuthorizationService authorizationService = authorizationService;

        [TempData]
        public string FormResult { get; set; } = "";

        [BindProperty]
        public Tournament Input { get; set; } = new Tournament { Name = "", OwnerId = "" };

        public IList<Tournament> Tournaments { get; set; } = [];

        public void OnGet()
        {
            var currentUserId = userManager.GetUserId(User);

            Tournaments = context
                .Tournaments.Where(tournament => tournament.OwnerId == currentUserId)
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var currentUserId = userManager.GetUserId(User);

            if (currentUserId is null)
            {
                return Unauthorized();
            }

            Input.OwnerId = currentUserId;

            var isAuthorized = await authorizationService.AuthorizeAsync(User, Input, "EditPolicy");

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            var tournament = new Tournament { Name = Input.Name, OwnerId = Input.OwnerId };

            context.Tournaments.Add(tournament);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    "Input.Name",
                    $"A tournament named '{tournament.Name}' already exists."
                );
                return Page();
            }

            FormResult = $"My tournament '{tournament.Name}' has been created.";

            Input.Name = "";
            Input.OwnerId = "";

            return RedirectToPage();
        }
    }
}
