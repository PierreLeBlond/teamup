namespace Webapp.Models;

public class Result
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public Team Team { get; set; } = null!;

    public int Value { get; set; } = 0;
}
