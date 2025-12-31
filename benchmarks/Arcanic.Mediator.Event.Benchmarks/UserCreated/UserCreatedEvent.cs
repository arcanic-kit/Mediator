using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Event.Benchmarks.UserCreated;

public class UserCreatedEvent : IEvent
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}