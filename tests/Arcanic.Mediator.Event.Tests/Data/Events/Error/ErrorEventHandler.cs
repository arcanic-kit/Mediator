using Arcanic.Mediator.Event.Abstractions.Handler;

namespace Arcanic.Mediator.Event.Tests.Data.Events.Error;

public class ErrorEventHandler : IEventHandler<ErrorEvent>
{
    public Task HandleAsync(ErrorEvent @event, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException(@event.ErrorMessage);
    }
}