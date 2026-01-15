using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Handler;
using Arcanic.Mediator.Query.Abstractions.Pipeline;

namespace Arcanic.Mediator.Query.Pipeline;

/// <summary>
/// Pipeline behavior that executes post-handler logic after a query has been processed.
/// Invokes all registered <see cref="IQueryPostHandler{TRequest}"/> instances for the given request.
/// </summary>
/// <typeparam name="TRequest">The type of the query request.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public class QueryPostHandlerPipelineBehavior<TRequest, TResponse> : IQueryPipelineBehavior<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    /// <summary>
    /// The collection of post-handler instances to be executed after the query handler.
    /// </summary>
    private readonly IEnumerable<IQueryPostHandler<TRequest>> _postHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryPostHandlerPipelineBehavior{TRequest,TResponse}"/> class.
    /// </summary>
    /// <param name="postHandlers">The collection of post-handler instances.</param>
    public QueryPostHandlerPipelineBehavior(IEnumerable<IQueryPostHandler<TRequest>> postHandlers)
        => _postHandlers = postHandlers;

    /// <summary>
    /// Handles the query by invoking the next delegate in the pipeline and then executing all post-handlers.
    /// </summary>
    /// <param name="message">The query request message.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The response from the next delegate in the pipeline.</returns>
    public async Task<TResponse> HandleAsync(TRequest message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var response = await next(cancellationToken).ConfigureAwait(false);

        foreach (var postHandler in _postHandlers)
        {
            await postHandler.HandleAsync(message, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }
}
