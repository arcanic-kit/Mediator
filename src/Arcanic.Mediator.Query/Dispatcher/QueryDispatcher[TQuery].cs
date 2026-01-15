using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Handler;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Query.Dispatcher;

/// <summary>
/// Concrete implementation of query dispatcher for a specific query and response type.
/// Handles the dispatch of the appropriate query handler and applies any registered pipeline behaviors.
/// </summary>
/// <typeparam name="TQuery">The type of the query, implementing <see cref="IQuery{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public class QueryDispatcher<TQuery, TResponse> : QueryDispatcherBase
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Dispatches the specified query using the provided service provider and cancellation token.
    /// Casts the to the appropriate query type and delegates to the strongly-typed dispatcher.
    /// </summary>
    /// <param name="query">The query to dispatch (as an object).</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation s.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with an optional result object.
    /// </returns>
    public override async Task<object?> DispatchAsync(object query, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await DispatchAsync((IQuery<TResponse>) query, serviceProvider, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Dispatches the specified query  using the provided service provider and cancellation token.
    /// Resolves the query handler and applies all registered pipeline behaviors in reverse order.
    /// </summary>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation s.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of type <typeparamref name="TResponse"/>.
    /// </returns>
    private Task<TResponse> DispatchAsync(IQuery<TResponse> query, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        // Defines the core handler delegate that invokes the query handler.
        Task<TResponse> Handler(CancellationToken t = default) => serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>()
            .HandleAsync((TQuery) query, t == default ? cancellationToken : t);

        return GetAllPipelineBehaviors<TQuery, TResponse>(serviceProvider)
            .Aggregate((PipelineDelegate<TResponse>) Handler,
                (next, pipeline) => (t) => pipeline.HandleAsync((TQuery) query, next, t == default ? cancellationToken : t))();
    }

}
