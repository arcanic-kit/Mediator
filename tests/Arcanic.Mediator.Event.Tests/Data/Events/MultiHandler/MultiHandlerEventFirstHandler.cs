using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Tests.Shared;

namespace Arcanic.Mediator.Event.Tests.Data.Events.MultiHandler;

public class MultiHandlerEventFirstHandler(ExecutedTypeTracker executedTypeTracker) : IEventHandler<MultiHandlerEvent>
{
    public Task HandleAsync(MultiHandlerEvent @event, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(GetType());
        return Task.CompletedTask;
    }
}
