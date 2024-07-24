using System.ComponentModel.DataAnnotations;

namespace Webapp.Models;

public class Tournament
{
    [Key]
    [StringLength(60, MinimumLength = 3)]
    public required string Name { get; set; }
}
