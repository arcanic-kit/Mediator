namespace Arcanic.Mediator.Request.Abstractions;

/// <summary>
/// Defines the contract for a mediator that routes requests (commands and queries) through their pipeline to a handler.
/// </summary>
/// <remarks>
/// Application code normally calls the typed <c>SendAsync</c> extension methods supplied by the command and query
/// packages, which are built on <see cref="DispatchAsync"/>. To fake the mediator in a test, implement
/// <see cref="DispatchAsync"/> and return the response the handler would produce.
/// </remarks>
public interface IMediator
{
    /// <summary>
    /// Dispatches a request through its pipeline behaviors, pre-handlers, handler and post-handlers.
    /// </summary>
    /// <param name="request">The request to dispatch.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// The handler's response, or <see cref="Arcanic.Mediator.Abstractions.Unit.Value"/> for requests without a response.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no registered module can dispatch the request type.</exception>
    Task<object?> DispatchAsync(IRequest request, CancellationToken cancellationToken = default);
}
