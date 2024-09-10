using System.ComponentModel.DataAnnotations.Schema;

namespace Webapp.Models;

public class Player
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public Guid TournamentId { get; set; }

    public Tournament Tournament { get; set; } = null!;

    public ICollection<Teammate> Teammates { get; set; } = [];

    [NotMapped]
    public int Score { get; set; } = 0;
}
