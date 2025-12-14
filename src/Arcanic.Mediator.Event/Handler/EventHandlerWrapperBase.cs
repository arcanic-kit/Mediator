namespace Arcanic.Mediator.Event.Handler;

public abstract class EventHandlerWrapperBase
{
    public abstract Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}