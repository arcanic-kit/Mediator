using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Dispatcher;
using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Request.Abstractions.Dispatcher;

namespace Arcanic.Mediator.Request.Abstractions;

/// <summary>
/// Provides extension methods for <see cref="IMediator"/> to support query processing functionality.
/// </summary>
public static class MediatorExtensions
{
    /// <summary>
    /// Sends a query asynchronously through the mediator and returns the query response.
    /// </summary>
    /// <typeparam name="TQueryResponse">The type of the query response.</typeparam>
    /// <param name="mediator">The mediator instance to send the query through.</param>
    /// <param name="query">The query to be processed.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous query operation. The task result contains the query response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the query dispatcher cannot be created for the specified query type.</exception>
    public static async Task<TQueryResponse> SendAsync<TQueryResponse>(this IMediator mediator, IQuery<TQueryResponse> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var queryType = query.GetType();

        // Retrieve or create dispatcher using thread-safe GetOrAdd
        var dispatcher = mediator.RequestDispatchers.GetOrAdd(queryType, static requestType =>
        {
            // Create typed dispatcher: QueryDispatcher<TQuery, TQueryResponse>
            var dispatcherType = typeof(QueryDispatcher<,>).MakeGenericType(requestType, typeof(TQueryResponse));
            var dispatcher = Activator.CreateInstance(dispatcherType) ?? throw new InvalidOperationException($"Could not create dispatcher type for {requestType}");
            return (RequestDispatcherBase)dispatcher;
        });

        // Execute query through dispatcher and return typed result
        var result = await dispatcher.DispatchAsync(query, mediator.ServiceProvider, cancellationToken);
        return (TQueryResponse)result!;
    }
}
