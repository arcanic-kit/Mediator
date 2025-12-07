using Arcanic.Mediator.Benchmarks.Models;
using MediatR;

namespace Arcanic.Mediator.Benchmarks.MediatR.Events;

/// <summary>
/// MediatR wrapper for UserCreatedEvent
/// </summary>
public record UserCreatedNotification(int Id, string Name, string Email, DateTime CreatedAt) : INotification;

/// <summary>
/// MediatR wrapper for UserUpdatedEvent
/// </summary>
public record UserUpdatedNotification(int Id, string OldName, string NewName, string OldEmail, string NewEmail, DateTime UpdatedAt) : INotification;

/// <summary>
/// MediatR wrapper for UserDeletedEvent
/// </summary>
public record UserDeletedNotification(int Id, string Reason, DateTime DeletedAt) : INotification;

/// <summary>
/// MediatR handler for UserCreatedEvent - Handler 1
/// </summary>
public class UserCreatedEmailHandler : INotificationHandler<UserCreatedNotification>
{
    public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        // Simulate sending email
        return Task.CompletedTask;
    }
}

/// <summary>
/// MediatR handler for UserCreatedEvent - Handler 2
/// </summary>
public class UserCreatedAuditHandler : INotificationHandler<UserCreatedNotification>
{
    public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        // Simulate audit logging
        return Task.CompletedTask;
    }
}

/// <summary>
/// MediatR handler for UserUpdatedEvent
/// </summary>
public class UserUpdatedHandler : INotificationHandler<UserUpdatedNotification>
{
    public Task Handle(UserUpdatedNotification notification, CancellationToken cancellationToken)
    {
        // Simulate handling user update
        return Task.CompletedTask;
    }
}

/// <summary>
/// MediatR handler for UserDeletedEvent
/// </summary>
public class UserDeletedHandler : INotificationHandler<UserDeletedNotification>
{
    public Task Handle(UserDeletedNotification notification, CancellationToken cancellationToken)
    {
        // Simulate handling user deletion
        return Task.CompletedTask;
    }
}