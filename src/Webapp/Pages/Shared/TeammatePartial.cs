using Webapp.Models;

namespace Webapp.Pages;

public class TeammatePartial
{
    public bool IsOwner = false;
    public required Tournament Tournament { get; set; }
    public required Player Player { get; set; }
    public required Game Game { get; set; }
    public required Team Team { get; set; }
    public Player? CurrentPlayer { get; set; } = null;

    public required Teammate Teammate { get; set; }
}
