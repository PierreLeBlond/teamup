using Webapp.Models;

namespace Webapp.Pages;

public class TeamPartial
{
    public bool IsOwner = false;
    public required Tournament Tournament { get; set; }
    public required Game Game { get; set; }
    public required Team Team { get; set; }
    public IList<Teammate> Teammates { get; set; } = [];
}
