namespace Arcanic.Mediator.Event.Handler;

/// <summary>
/// Serves as the abstract base class for all event handler wrappers.
/// Provides a contract for handling events with dependency injection support,
/// allowing derived classes to implement event handling logic for specific event types.
/// </summary>
public abstract class EventHandlerWrapperBase
{
    /// <summary>
    /// Handles the specified event asynchronously using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="request">The event object to handle.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing an optional result object.
    /// </returns>
    public abstract Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}