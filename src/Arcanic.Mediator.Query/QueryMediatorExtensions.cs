using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Request.Abstractions;

/// <summary>
/// Provides typed query-sending methods on <see cref="IMediator"/>.
/// </summary>
public static class QueryMediatorExtensions
{
    /// <summary>
    /// Sends a query through its pipeline: pipeline behaviors, pre-handlers, the query handler and post-handlers.
    /// </summary>
    /// <typeparam name="TQueryResponse">The type of result the query produces.</typeparam>
    /// <param name="mediator">The mediator to send the query through.</param>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task whose result is the query handler's response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="mediator"/> or <paramref name="query"/> is <c>null</c>.</exception>
    public static async Task<TQueryResponse> SendAsync<TQueryResponse>(this IMediator mediator, IQuery<TQueryResponse> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(query);

        var result = await mediator.DispatchAsync(query, cancellationToken).ConfigureAwait(false);

        return (TQueryResponse)result!;
    }
}
