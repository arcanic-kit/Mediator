using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Benchmarks.Events.UserDeleted;

public class UserDeletedEvent : IEvent
{
    public int Id { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
}