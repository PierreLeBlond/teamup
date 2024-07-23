using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webapp.Data;
using Webapp.Models;

namespace Webapp.Pages
{
    public class TournamentListModel(Context context) : PageModel
    {
        private readonly Context context = context;
        public IList<Tournament> TournamentList { get; set; } = default!;

        public void OnGet()
        {
            TournamentList = [.. context.Tournaments];
        }
    }
}
