using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Pipeline;

namespace Arcanic.Mediator.Command.Pipeline;

public class CommandPreHandlerPipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// The collection of pre-handler instances to be executed before the query handler.
    /// </summary>
    private readonly IEnumerable<ICommandPreHandler<TRequest>> _preHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandPreHandlerPipelineBehavior{TRequest,TResponse}"/> class.
    /// </summary>
    /// <param name="preHandlers">The collection of pre-handler instances.</param>
    public CommandPreHandlerPipelineBehavior(IEnumerable<ICommandPreHandler<TRequest>> preHandlers)
        => _preHandlers = preHandlers;

    /// <summary>
    /// Handles the query by executing all pre-handlers before invoking the next delegate in the pipeline.
    /// </summary>
    /// <param name="message">The query request message.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The response from the next delegate in the pipeline.</returns>
    public async Task<TResponse> HandleAsync(TRequest message, RequestPipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        foreach (var preHandler in _preHandlers)
        {
            await preHandler.HandleAsync(message, cancellationToken).ConfigureAwait(false);
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
