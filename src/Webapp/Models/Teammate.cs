namespace Webapp.Models;

public class Teammate
{
    public Guid Id { get; set; }

    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public Guid PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public int Bonus { get; set; } = 0;
    public int Malus { get; set; } = 0;
}
