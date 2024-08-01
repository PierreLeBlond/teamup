using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Webapp.Models;

public class Tournament
{
    [Key]
    [StringLength(60, MinimumLength = 3)]
    public required string Name { get; set; }

    [ValidateNever]
    public required string OwnerId { get; set; }

    public ICollection<Player> Players { get; set; } = [];
}
