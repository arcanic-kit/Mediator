namespace Arcanic.Mediator.Query.Dispatcher;

/// <summary>
/// Abstract base class for query dispatchers.
/// Provides an interface for dispatching queries with dynamic query types and dependency injection.
/// </summary>
public abstract class QueryDispatcherBase
{
    /// <summary>
    /// Dispatches the specified query using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with an optional result object.
    /// </returns>
    public abstract Task<object?> DispatchAsync(object query, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}