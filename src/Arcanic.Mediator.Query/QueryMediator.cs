using Arcanic.Mediator.Query.Abstractions;
using System.Collections.Concurrent;
using Arcanic.Mediator.Query.Handler;

namespace Arcanic.Mediator.Query;

/// <summary>
/// Provides query mediation capabilities by coordinating the execution of queries
/// through the underlying message mediator framework.
/// High-performance version that includes fast paths for common scenarios.
/// </summary>
public class QueryMediator : IQueryMediator
{
    /// <summary>
    /// The service provider used for dependency injection and handler resolution.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// A thread-safe cache of query handler wrappers, keyed by query type.
    /// Used to avoid repeated reflection and instantiation costs.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, QueryHandlerWrapperBase> QueryHandlers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryMediator"/> class with the specified message mediator.
    /// </summary>
    /// <param name="serviceProvider">The service provider used for dependency injection and handler resolution.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceProvider"/> is null.</exception>
    public QueryMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Asynchronously executes a query and returns the result using a single handler strategy.
    /// Uses fast path optimization when no pipeline behaviors are registered.
    /// </summary>
    /// <typeparam name="TQueryResponse">The type of result returned by the query.</typeparam>
    /// <param name="query">The query to execute.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>
    /// A task representing the asynchronous query execution with the result of type <typeparamref name="TQueryResponse"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the handler wrapper cannot be created for the query type.</exception>
    public Task<TQueryResponse> SendAsync<TQueryResponse>(IQuery<TQueryResponse> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var queryType = query.GetType();

        // Retrieve or create a handler wrapper for the query type.
        var handler = (QueryHandlerWrapper<TQueryResponse>)QueryHandlers.GetOrAdd(queryType, static requestType =>
        {
            var wrapperType = typeof(QueryHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TQueryResponse));
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
            return (QueryHandlerWrapperBase)wrapper;
        });

        // Delegate the handling of the query to the resolved handler.
        return handler.Handle(query, _serviceProvider, cancellationToken);
    }
}
