namespace Webapp.Models;

public class Game
{
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public Tournament Tournament { get; set; } = null!;

    public ICollection<Reward> Rewards { get; set; } = [];

    public required string Name { get; set; }

    public required bool ShouldMaximizeScore { get; set; } = true;

    public required int NumberOfTeams { get; set; }
}
