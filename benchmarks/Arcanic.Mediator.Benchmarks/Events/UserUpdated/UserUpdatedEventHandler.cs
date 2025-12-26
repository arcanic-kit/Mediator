using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Handler;

namespace Arcanic.Mediator.Benchmarks.Events.UserUpdated;

public class UserUpdatedEventHandler : IEventHandler<UserUpdatedEvent>
{
    public Task HandleAsync(UserUpdatedEvent request, CancellationToken cancellationToken = default)
    {
        // Simulate handling user update
        return Task.CompletedTask;
    }
}