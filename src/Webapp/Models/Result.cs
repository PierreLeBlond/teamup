namespace Webapp.Models;

public class Result
{
    public Guid Id { get; set; }

    public Guid TeamId { get; set; }

    public Team Team { get; set; } = null!;

    public int Value { get; set; } = 0;
}
