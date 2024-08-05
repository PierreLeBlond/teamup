using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Webapp.Models;

public class Reward
{
    [ValidateNever]
    public Guid Id { get; set; }

    [ValidateNever]
    public required Guid GameId { get; set; }

    public required int Value { get; set; } = 0;
}
