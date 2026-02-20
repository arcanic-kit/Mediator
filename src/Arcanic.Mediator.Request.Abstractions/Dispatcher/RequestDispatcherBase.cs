namespace Arcanic.Mediator.Request.Abstractions.Dispatcher;

public abstract class RequestDispatcherBase
{
    public abstract Task<object?> DispatchAsync(object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken
    );
}
