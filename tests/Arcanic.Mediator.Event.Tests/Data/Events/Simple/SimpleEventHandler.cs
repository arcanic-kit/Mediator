using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Event.Tests.Utils;

namespace Arcanic.Mediator.Event.Tests.Data.Events.Simple;

public class SimpleEventHandler(ExecutedTypeTracker executedTypeTracker) : IEventHandler<SimpleEvent>
{
    public Task HandleAsync(SimpleEvent @event, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(this.GetType());
        return Task.CompletedTask;
    }
}