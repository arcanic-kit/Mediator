using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Handler;
using Arcanic.Mediator.Query.Abstractions.Pipeline;

namespace Arcanic.Mediator.Query.Pipeline;

/// <summary>
/// Pipeline behavior that executes pre-handler logic before a query is processed.
/// Invokes all registered <see cref="IQueryPreHandler{TRequest}"/> instances for the given request.
/// </summary>
/// <typeparam name="TRequest">The type of the query request.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public class QueryPreHandlerPipelineBehavior<TRequest, TResponse> : IQueryPipelineBehavior<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    /// <summary>
    /// The collection of pre-handler instances to be executed before the query handler.
    /// </summary>
    private readonly IEnumerable<IQueryPreHandler<TRequest>> _preHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryPreHandlerPipelineBehavior{TRequest,TResponse}"/> class.
    /// </summary>
    /// <param name="preHandlers">The collection of pre-handler instances.</param>
    public QueryPreHandlerPipelineBehavior(IEnumerable<IQueryPreHandler<TRequest>> preHandlers)
        => _preHandlers = preHandlers;

    /// <summary>
    /// Handles the query by executing all pre-handlers before invoking the next delegate in the pipeline.
    /// </summary>
    /// <param name="message">The query request message.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The response from the next delegate in the pipeline.</returns>
    public async Task<TResponse> HandleAsync(TRequest message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        foreach (var preHandler in _preHandlers)
        {
            await preHandler.HandleAsync(message, cancellationToken).ConfigureAwait(false);
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
