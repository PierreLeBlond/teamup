namespace Webapp.Models;

public class Game
{
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public Tournament Tournament { get; set; } = null!;

    public ICollection<Reward> Rewards { get; set; } = [];
    public ICollection<Team> Teams { get; set; } = [];

    public required string Name { get; set; }

    public required bool ShouldMaximizeScore { get; set; } = true;

    public required int NumberOfTeams { get; set; }
}
