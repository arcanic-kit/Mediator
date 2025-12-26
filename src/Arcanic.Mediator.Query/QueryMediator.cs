using System.Collections.Concurrent;
using Arcanic.Mediator.Query.Dispatcher;
using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Query;

/// <summary>
/// Provides query mediation capabilities with performance optimizations.
/// Uses a cached dispatcher pattern to minimize reflection overhead.
/// </summary>
public class QueryMediator : IQueryMediator
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Thread-safe cache of query dispatchers to avoid reflection overhead.
    /// Keyed by query type for efficient lookup and reuse.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, QueryDispatcherBase> QueryDispatchers = new();

    /// <summary>
    /// Initializes a new instance of the QueryMediator.
    /// </summary>
    /// <param name="serviceProvider">Service provider for dependency injection and handler resolution.</param>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider is null.</exception>
    public QueryMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Sends a query asynchronously and returns the response.
    /// Utilizes cached dispatchers for optimal performance.
    /// </summary>
    /// <typeparam name="TQueryResponse">The expected response type.</typeparam>
    /// <param name="query">The query to execute.</param>
    /// <param name="cancellationToken">Cancellation token for operation cancellation.</param>
    /// <returns>The query response of type TQueryResponse.</returns>
    /// <exception cref="ArgumentNullException">Thrown when query is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when dispatcher creation fails.</exception>
    public async Task<TQueryResponse> SendAsync<TQueryResponse>(IQuery<TQueryResponse> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var queryType = query.GetType();

        // Retrieve or create dispatcher using thread-safe GetOrAdd
        var dispatcher = QueryDispatchers.GetOrAdd(queryType, static requestType =>
        {
            // Create typed dispatcher: QueryDispatcher<TQuery, TQueryResponse>
            var dispatcherType = typeof(QueryDispatcher<,>).MakeGenericType(requestType, typeof(TQueryResponse));
            var dispatcher = Activator.CreateInstance(dispatcherType) ?? throw new InvalidOperationException($"Could not create dispatcher type for {requestType}");
            return (QueryDispatcherBase)dispatcher;
        });

        // Execute query through dispatcher and return typed result
        var result = await dispatcher.DispatchAsync(query, _serviceProvider, cancellationToken);
        return (TQueryResponse)result!;
    }
}
