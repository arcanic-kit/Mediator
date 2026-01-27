using Arcanic.Mediator.Event.Abstractions.Handler;

namespace Arcanic.Mediator.Event.Tests.Data.Events.Cancellable;

public class CancellableEventHandler : IEventHandler<CancellableEvent>
{
    public async Task HandleAsync(CancellableEvent @event, CancellationToken cancellationToken = default)
    {
        await Task.Delay(@event.DelayMilliseconds, cancellationToken);
    }
}