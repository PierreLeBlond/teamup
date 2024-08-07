namespace Webapp.Models;

public class Reward
{
    public Guid Id { get; set; }

    public Guid GameId { get; set; }

    public Game Game { get; set; } = null!;

    public required int Value { get; set; } = 0;
}
