using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Event.Handler;

/// <summary>
/// Serves as an abstract base class for event handler wrappers,
/// providing a contract for handling events with dependency injection support.
/// </summary>
public abstract class EventHandlerWrapper : EventHandlerWrapperBase
{
    /// <summary>
    /// Handles the specified event asynchronously using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="request">The event to handle.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation, containing a <see cref="Unit"/> result.</returns>
    public abstract Task<Unit> Handle(IEvent request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}