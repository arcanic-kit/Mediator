using Arcanic.Mediator.Benchmarks.Models;
using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Benchmarks.Arcanic.Events;

/// <summary>
/// Arcanic event wrapper for UserCreatedEvent
/// </summary>
public class UserCreatedArcanicEvent : IEvent
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Arcanic event wrapper for UserUpdatedEvent
/// </summary>
public class UserUpdatedArcanicEvent : IEvent
{
    public int Id { get; init; }
    public string OldName { get; init; } = string.Empty;
    public string NewName { get; init; } = string.Empty;
    public string OldEmail { get; init; } = string.Empty;
    public string NewEmail { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Arcanic event wrapper for UserDeletedEvent
/// </summary>
public class UserDeletedArcanicEvent : IEvent
{
    public int Id { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Arcanic handler for UserCreatedEvent - Handler 1
/// </summary>
public class UserCreatedEmailArcanicHandler : IEventHandler<UserCreatedArcanicEvent>
{
    public Task HandleAsync(UserCreatedArcanicEvent request, CancellationToken cancellationToken = default)
    {
        // Simulate sending email
        return Task.CompletedTask;
    }
}

/// <summary>
/// Arcanic handler for UserCreatedEvent - Handler 2
/// </summary>
public class UserCreatedAuditArcanicHandler : IEventHandler<UserCreatedArcanicEvent>
{
    public Task HandleAsync(UserCreatedArcanicEvent request, CancellationToken cancellationToken = default)
    {
        // Simulate audit logging
        return Task.CompletedTask;
    }
}

/// <summary>
/// Arcanic handler for UserUpdatedEvent
/// </summary>
public class UserUpdatedArcanicHandler : IEventHandler<UserUpdatedArcanicEvent>
{
    public Task HandleAsync(UserUpdatedArcanicEvent request, CancellationToken cancellationToken = default)
    {
        // Simulate handling user update
        return Task.CompletedTask;
    }
}

/// <summary>
/// Arcanic handler for UserDeletedEvent
/// </summary>
public class UserDeletedArcanicHandler : IEventHandler<UserDeletedArcanicEvent>
{
    public Task HandleAsync(UserDeletedArcanicEvent request, CancellationToken cancellationToken = default)
    {
        // Simulate handling user deletion
        return Task.CompletedTask;
    }
}