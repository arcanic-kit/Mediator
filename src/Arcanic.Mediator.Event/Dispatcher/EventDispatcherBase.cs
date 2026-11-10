namespace Arcanic.Mediator.Event.Dispatcher;

/// <summary>
/// Abstract base class for event dispatchers.
/// Provides an interface for dispatching events with dynamic event types and dependency injection.
/// </summary>
public abstract class EventDispatcherBase
{
    /// <summary>
    /// Dispatches the specified event using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="event">The event to dispatch.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with an optional result object.
    /// </returns>
    public abstract Task<object?> DispatchAsync(object @event, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}
