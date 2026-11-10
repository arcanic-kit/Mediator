namespace Arcanic.Mediator.Request.Abstractions;

/// <summary>
/// Represents a request (command or query) that produces a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <remarks>
/// <see cref="IMediator.SendAsync{TResponse}(IRequest{TResponse}, CancellationToken)"/> infers
/// <typeparamref name="TResponse"/> from this interface, so sending a request returns a typed response.
/// </remarks>
/// <typeparam name="TResponse">The type of response the request produces.</typeparam>
public interface IRequest<TResponse> : IRequest;
