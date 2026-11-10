using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Event.Tests.Utils;

namespace Arcanic.Mediator.Event.Tests.Data.Events.MultiHandler;

public class MultiHandlerEventSecondHandler(ExecutedTypeTracker executedTypeTracker) : IEventHandler<MultiHandlerEvent>
{
    public Task HandleAsync(MultiHandlerEvent @event, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(GetType());
        return Task.CompletedTask;
    }
}
