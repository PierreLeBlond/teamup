using System.ComponentModel.DataAnnotations;

namespace Webapp.Models;

public class Tournament
{
    [Key]
    public required string Name { get; set; }
}
