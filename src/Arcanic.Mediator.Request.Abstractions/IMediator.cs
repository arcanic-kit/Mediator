namespace Arcanic.Mediator.Request.Abstractions;

/// <summary>
/// Sends requests (commands and queries) through their pipeline to a handler.
/// </summary>
/// <remarks>
/// Both methods are interface members, so <see cref="IMediator"/> can be mocked directly with libraries such as
/// Moq or NSubstitute, or replaced by a hand-written fake.
/// </remarks>
public interface IMediator
{
    /// <summary>
    /// Sends a request that produces a response (a command with a result, or a query) through its
    /// pipeline behaviors, pre-handlers, handler and post-handlers.
    /// </summary>
    /// <typeparam name="TResponse">The type of response, inferred from the request.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task whose result is the handler's response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no registered module can handle the request type.</exception>
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request that produces no response (a command without a result) through its
    /// pipeline behaviors, pre-handlers, handler and post-handlers.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the request has been handled.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no registered module can handle the request type.</exception>
    Task SendAsync(IRequest request, CancellationToken cancellationToken = default);
}
