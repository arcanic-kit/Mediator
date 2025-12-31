using System;
using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Event.Benchmarks.UserDeleted;

public class UserDeletedEvent : IEvent
{
    public int Id { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
}