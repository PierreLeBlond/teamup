using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Webapp.Models;

public class Player
{
    [ValidateNever]
    public Guid Id { get; set; }

    [StringLength(
        60,
        MinimumLength = 3,
        ErrorMessage = "Thou must provide a name between 3 and 60 characters."
    )]
    public required string Name { get; set; }

    [ValidateNever]
    public required string TournamentId { get; set; }

    [ValidateNever]
    public Tournament Tournament { get; set; } = null!;
}