using Arcanic.Mediator.Pipeline;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Handler;
using Arcanic.Mediator.Query.Abstractions.Pipeline;

namespace Arcanic.Mediator.Query.Pipeline;

/// <summary>
/// Pipeline behavior that runs every registered <see cref="IQueryPreHandler{T}"/> for the pre-handler stage of a query.
/// </summary>
/// <typeparam name="TRequest">The query type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class QueryPreHandlerPipelineBehavior<TRequest, TResponse>
    : PreHandlerPipelineBehaviorBase<TRequest, TResponse, IQueryPreHandler<TRequest>>, IQueryPipelineBehavior<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryPreHandlerPipelineBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="preHandlers">The pre-handlers registered for <typeparamref name="TRequest"/>.</param>
    public QueryPreHandlerPipelineBehavior(IEnumerable<IQueryPreHandler<TRequest>> preHandlers)
        : base(preHandlers)
    {
    }
}
