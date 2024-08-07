namespace Webapp.Models;

public class Tournament
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string OwnerId { get; set; }

    public ICollection<Player> Players { get; set; } = [];
    public ICollection<Game> Games { get; set; } = [];
}
