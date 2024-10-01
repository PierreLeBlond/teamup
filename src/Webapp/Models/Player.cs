using System.ComponentModel.DataAnnotations.Schema;

namespace Webapp.Models;

public class Player
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int TournamentId { get; set; }

    public Tournament Tournament { get; set; } = null!;

    public ICollection<Teammate> Teammates { get; set; } = [];

    [NotMapped]
    public int Score { get; set; } = 0;
}
