using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Webapp.Models;

public class Reward
{
    [ValidateNever]
    public Guid Id { get; set; }

    [ValidateNever]
    public Guid GameId { get; set; }

    [ValidateNever]
    public Game Game { get; set; } = null!;

    public required int Value { get; set; } = 0;
}
