using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Webapp.Models;

public class Game
{
    [ValidateNever]
    public Guid Id { get; set; }

    [ValidateNever]
    public Guid TournamentId { get; set; }

    [ValidateNever]
    public Tournament Tournament { get; set; } = null!;

    public ICollection<Reward> Rewards { get; set; } = [];

    [Required(ErrorMessage = "Thou must provide a name between 3 and 60 characters.")]
    [StringLength(
        60,
        MinimumLength = 3,
        ErrorMessage = "Thou must provide a name between 3 and 60 characters."
    )]
    public required string Name { get; set; }

    public required bool ShouldMaximizeScore { get; set; } = true;

    [Required(ErrorMessage = "Thou must provide a number of teams between 1 and 300.")]
    [Range(1, 300, ErrorMessage = "Thou must provide a number of teams between 1 and 300.")]
    public required int NumberOfTeams { get; set; }
}
