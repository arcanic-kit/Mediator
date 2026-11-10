using Arcanic.Mediator.Abstractions.Handler.Post;
using Arcanic.Mediator.Abstractions.Pipeline;

namespace Arcanic.Mediator.Pipeline;

/// <summary>
/// Base pipeline behavior that continues the pipeline, then runs every registered post-handler in registration order.
/// Module-specific behaviors (command, query, event) derive from this and supply their post-handler interface.
/// </summary>
/// <typeparam name="TMessage">The message type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <typeparam name="TPostHandler">The module post-handler interface.</typeparam>
public abstract class PostHandlerPipelineBehaviorBase<TMessage, TResponse, TPostHandler>
    where TMessage : notnull
    where TPostHandler : IAsyncMessagePostHandler<TMessage>
{
    private readonly IEnumerable<TPostHandler> _postHandlers;

    /// <summary>
    /// Initializes a new instance with the post-handlers resolved for <typeparamref name="TMessage"/>.
    /// </summary>
    /// <param name="postHandlers">The post-handlers to run.</param>
    protected PostHandlerPipelineBehaviorBase(IEnumerable<TPostHandler> postHandlers)
        => _postHandlers = postHandlers;

    /// <summary>
    /// Invokes the next delegate in the pipeline, then runs all post-handlers.
    /// </summary>
    /// <param name="message">The message being processed.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The response produced by the rest of the pipeline.</returns>
    public async Task<TResponse> HandleAsync(TMessage message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var response = await next(cancellationToken).ConfigureAwait(false);

        foreach (var postHandler in _postHandlers)
        {
            await postHandler.HandleAsync(message, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }
}
