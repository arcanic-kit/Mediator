using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Benchmarks.Events.UserCreated;

public class UserCreatedEmailEventHandler : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent request, CancellationToken cancellationToken = default)
    {
        // Simulate sending email
        return Task.CompletedTask;
    }
}
