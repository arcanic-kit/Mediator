using Arcanic.Mediator.Pipeline;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Handler;
using Arcanic.Mediator.Query.Abstractions.Pipeline;

namespace Arcanic.Mediator.Query.Pipeline;

/// <summary>
/// Pipeline behavior that runs every registered <see cref="IQueryPostHandler{T}"/> for the post-handler stage of a query.
/// </summary>
/// <typeparam name="TRequest">The query type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class QueryPostHandlerPipelineBehavior<TRequest, TResponse>
    : PostHandlerPipelineBehaviorBase<TRequest, TResponse, IQueryPostHandler<TRequest>>, IQueryPipelineBehavior<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryPostHandlerPipelineBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="postHandlers">The post-handlers registered for <typeparamref name="TRequest"/>.</param>
    public QueryPostHandlerPipelineBehavior(IEnumerable<IQueryPostHandler<TRequest>> postHandlers)
        : base(postHandlers)
    {
    }
}
