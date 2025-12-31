using System;
using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Event.Benchmarks.UserUpdated;

public class UserUpdatedEvent : IEvent
{
    public int Id { get; init; }
    public string OldName { get; init; } = string.Empty;
    public string NewName { get; init; } = string.Empty;
    public string OldEmail { get; init; } = string.Empty;
    public string NewEmail { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}