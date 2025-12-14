using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Event.Handler;

public abstract class EventHandlerWrapper : EventHandlerWrapperBase
{
    public abstract Task<Unit> Handle(IEvent request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}