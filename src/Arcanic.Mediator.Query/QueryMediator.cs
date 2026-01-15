using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Arcanic.Mediator.Query.Dispatcher;
using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Query;

/// <summary>
/// Provides query mediation capabilities with performance optimizations.
/// Uses a cached dispatcher pattern to minimize reflection overhead by maintaining
/// a thread-safe cache of query dispatchers for efficient query processing.
/// </summary>
public class QueryMediator : IQueryMediator
{
    /// <summary>
    /// The service provider used for dependency injection and handler resolution.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Thread-safe cache of query dispatchers to avoid reflection overhead.
    /// Keyed by query type for efficient lookup and reuse across multiple query executions.
    /// This static cache ensures that dispatcher instances are shared across all QueryMediator instances
    /// to maximize performance benefits.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, QueryDispatcherBase> QueryDispatchers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryMediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency injection and handler resolution.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceProvider"/> is null.</exception>
    public QueryMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Sends a query asynchronously and returns the response.
    /// Utilizes cached dispatchers for optimal performance by avoiding repeated reflection operations.
    /// The method first retrieves or creates a dispatcher for the query type, then executes the query
    /// through the appropriate pipeline including any registered pre-handlers, main handler, and post-handlers.
    /// </summary>
    /// <typeparam name="TQueryResponse">The expected response type from the query execution.</typeparam>
    /// <param name="query">The query to execute. Must implement <see cref="IQuery{TQueryResponse}"/>.</param>
    /// <param name="cancellationToken">Optional cancellation token for operation cancellation. Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation, containing the query response of type <typeparamref name="TQueryResponse"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when dispatcher creation fails or when no appropriate handler is found for the query type.</exception>
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
