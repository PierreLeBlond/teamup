using Webapp.Models;

namespace Webapp.Pages;

public class PlayerPartial
{
    public required Player Player { get; set; }
    public Player? CurrentPlayer { get; set; } = null;
}
