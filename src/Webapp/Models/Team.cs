using System.ComponentModel.DataAnnotations.Schema;

namespace Webapp.Models;

public class Team
{
    public int Id { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    public required int Number { get; set; }
    public int Bonus { get; set; } = 0;
    public int Malus { get; set; } = 0;

    public Result? Result { get; set; } = null;

    [NotMapped]
    public int Score { get; set; } = 0;

    [NotMapped]
    public int? Rank { get; set; } = null;

    public ICollection<Teammate> Teammates { get; set; } = [];
}
