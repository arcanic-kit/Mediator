using Arcanic.Mediator.Event.Abstractions.Handler;

namespace Arcanic.Mediator.Event.Benchmarks.UserUpdated;

public class UserUpdatedEventHandler : IEventHandler<UserUpdatedEvent>
{
    public Task HandleAsync(UserUpdatedEvent request, CancellationToken cancellationToken = default)
    {
        // Simulate handling user update
        return Task.CompletedTask;
    }
}