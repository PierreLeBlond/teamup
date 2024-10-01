namespace Webapp.Models;

public class Tournament
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string OwnerName { get; set; }

    public ICollection<Player> Players { get; set; } = [];
    public ICollection<Game> Games { get; set; } = [];
}
