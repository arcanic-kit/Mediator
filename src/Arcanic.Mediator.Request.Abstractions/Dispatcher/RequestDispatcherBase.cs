namespace Arcanic.Mediator.Request.Abstractions.Dispatcher;

/// <summary>
/// Provides the abstract base class for request dispatchers in the mediator pattern.
/// Derived classes must implement the request dispatching logic for specific request types.
/// </summary>
public abstract class RequestDispatcherBase
{
    /// <summary>
    /// Dispatches a request asynchronously to its appropriate handler.
    /// </summary>
    /// <param name="request">The request object to be dispatched and processed.</param>
    /// <param name="serviceProvider">The service provider used for dependency resolution and handler instantiation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous dispatch operation. The task result contains the response from the handler, or null if no response is expected.</returns>
    public abstract Task<object?> DispatchAsync(object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken
    );
}
