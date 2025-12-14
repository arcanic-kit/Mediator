using Arcanic.Mediator.Pipeline;
using Arcanic.Mediator.Query.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Query.Handler;

/// <summary>
/// Concrete implementation of <see cref="QueryHandlerWrapper{TResponse}"/> for a specific query and response type.
/// Handles the invocation of the appropriate query handler and applies any registered pipeline behaviors.
/// </summary>
/// <typeparam name="TQuery">The type of the query, implementing <see cref="IQuery{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public class QueryHandlerWrapperImpl<TQuery, TResponse> : QueryHandlerWrapper<TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Handles the specified query request using the provided service provider and cancellation token.
    /// Casts the request to the appropriate query type and delegates to the strongly-typed handler.
    /// </summary>
    /// <param name="request">The query request to handle (as an object).</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with an optional result object.
    /// </returns>
    public override async Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await Handle((IQuery<TResponse>) request, serviceProvider, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Handles the specified query request using the provided service provider and cancellation token.
    /// Resolves the query handler and applies all registered pipeline behaviors in reverse order.
    /// </summary>
    /// <param name="request">The query request to handle.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of type <typeparamref name="TResponse"/>.
    /// </returns>
    public override Task<TResponse> Handle(IQuery<TResponse> request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        // Defines the core handler delegate that invokes the query handler.
        Task<TResponse> Handler(CancellationToken t = default) => serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>()
            .HandleAsync((TQuery) request, t == default ? cancellationToken : t);

        // Applies all pipeline behaviors in reverse order, wrapping the handler.
        return serviceProvider
            .GetServices<IRequestPipelineBehavior<TQuery, TResponse>>()
            .Reverse()
            .Aggregate((RequestPipelineDelegate<TResponse>) Handler,
                (next, pipeline) => (t) => pipeline.HandleAsync((TQuery) request, next, t == default ? cancellationToken : t))();
    }
}
