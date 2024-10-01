namespace Webapp.Models;

public class Teammate
{
    public int Id { get; set; }

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public int Bonus { get; set; } = 0;
    public int Malus { get; set; } = 0;
}
