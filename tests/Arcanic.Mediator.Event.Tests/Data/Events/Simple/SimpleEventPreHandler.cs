using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Tests.Shared;

namespace Arcanic.Mediator.Event.Tests.Data.Events.Simple;

public class SimpleEventPreHandler(ExecutedTypeTracker executedTypeTracker) : IEventPreHandler<SimpleEvent>
{
    public Task HandleAsync(SimpleEvent @event, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(this.GetType());
        return Task.CompletedTask;
    }
}