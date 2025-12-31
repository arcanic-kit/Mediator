using System.Threading;
using System.Threading.Tasks;
using Arcanic.Mediator.Event.Abstractions.Handler;

namespace Arcanic.Mediator.Event.Benchmarks.UserCreated;

public class UserCreatedEmailEventHandler : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent request, CancellationToken cancellationToken = default)
    {
        // Simulate sending email
        return Task.CompletedTask;
    }
}
