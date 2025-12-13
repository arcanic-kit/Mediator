using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Query.Handler;

/// <summary>
/// Abstract generic base class for query handler wrappers.
/// Provides an interface for handling queries of a specific response type,
/// enabling dynamic invocation and dependency injection.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public abstract class QueryHandlerWrapper<TResponse> : QueryHandlerWrapperBase
{
    /// <summary>
    /// Handles the specified query request using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="request">The query request to handle.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of type <typeparamref name="TResponse"/>.
    /// </returns>
    public abstract Task<TResponse> Handle(IQuery<TResponse> request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}