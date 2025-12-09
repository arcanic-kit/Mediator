using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Benchmarks.Events.UserCreated;

public class UserCreatedAuditEventHandler : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent request, CancellationToken cancellationToken = default)
    {
        // Simulate audit logging
        return Task.CompletedTask;
    }
}
