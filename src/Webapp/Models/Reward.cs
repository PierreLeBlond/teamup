namespace Webapp.Models;

public class Reward
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public Game Game { get; set; } = null!;

    public required int Value { get; set; } = 0;
}
