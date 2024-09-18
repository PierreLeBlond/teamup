using Webapp.Models;

namespace Webapp.Pages;

public class TeamPartial
{
    public bool IsOwner = false;
    public required Tournament Tournament { get; set; }
    public required Game Game { get; set; }
    public int? PreviousRank { get; set; } = null;
    public required Team Team { get; set; }
    public Player? CurrentPlayer { get; set; } = null;
    public Team? CurrentTeam { get; set; } = null;
}
