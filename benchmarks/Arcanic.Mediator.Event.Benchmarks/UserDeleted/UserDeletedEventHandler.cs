using Arcanic.Mediator.Event.Abstractions.Handler;

namespace Arcanic.Mediator.Event.Benchmarks.UserDeleted;

public class UserDeletedEventHandler : IEventHandler<UserDeletedEvent>
{
    public Task HandleAsync(UserDeletedEvent request, CancellationToken cancellationToken = default)
    {
        // Simulate handling user deletion
        return Task.CompletedTask;
    }
}