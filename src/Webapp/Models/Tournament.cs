using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Webapp.Models;

public class Tournament
{
    [ValidateNever]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Thou must provide a name between 3 and 60 characters.")]
    [StringLength(
        60,
        MinimumLength = 3,
        ErrorMessage = "Thou must provide a name between 3 and 60 characters."
    )]
    public required string Name { get; set; }

    [ValidateNever]
    public required string OwnerId { get; set; }

    public ICollection<Player> Players { get; set; } = [];
    public ICollection<Game> Games { get; set; } = [];
}
