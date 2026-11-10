using Arcanic.Mediator.Abstractions.Handler.Pre;
using Arcanic.Mediator.Abstractions.Pipeline;

namespace Arcanic.Mediator.Pipeline;

/// <summary>
/// Base pipeline behavior that runs every registered pre-handler, in registration order, before continuing the pipeline.
/// Module-specific behaviors (command, query, event) derive from this and supply their pre-handler interface.
/// </summary>
/// <typeparam name="TMessage">The message type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <typeparam name="TPreHandler">The module pre-handler interface.</typeparam>
public abstract class PreHandlerPipelineBehaviorBase<TMessage, TResponse, TPreHandler>
    where TMessage : notnull
    where TPreHandler : IAsyncMessagePreHandler<TMessage>
{
    private readonly IEnumerable<TPreHandler> _preHandlers;

    /// <summary>
    /// Initializes a new instance with the pre-handlers resolved for <typeparamref name="TMessage"/>.
    /// </summary>
    /// <param name="preHandlers">The pre-handlers to run.</param>
    protected PreHandlerPipelineBehaviorBase(IEnumerable<TPreHandler> preHandlers)
        => _preHandlers = preHandlers;

    /// <summary>
    /// Runs all pre-handlers, then invokes the next delegate in the pipeline.
    /// </summary>
    /// <param name="message">The message being processed.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The response produced by the rest of the pipeline.</returns>
    public async Task<TResponse> HandleAsync(TMessage message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        foreach (var preHandler in _preHandlers)
        {
            await preHandler.HandleAsync(message, cancellationToken).ConfigureAwait(false);
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
