namespace Arcanic.Mediator.Abstractions.Pipeline;

/// <summary>
/// Defines a pipeline behavior that can intercept and process requests in the mediator pipeline.
/// </summary>
/// <typeparam name="TRequest">The type of the request. Must be a non-null reference type.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the request handler.</typeparam>
/// <remarks>
/// This interface extends <see cref="IPipelineBehavior{TRequest, TResponse}"/> to provide
/// request-specific pipeline behavior functionality. Pipeline behaviors allow for cross-cutting
/// concerns such as logging, validation, caching, or transaction handling to be applied
/// to requests before they reach their handlers.
/// </remarks>
public interface IRequestPipelineBehavior<in TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest { }