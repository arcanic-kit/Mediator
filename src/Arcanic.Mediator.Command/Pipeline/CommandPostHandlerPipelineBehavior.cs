using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Pipeline;

namespace Arcanic.Mediator.Command.Pipeline;

public class CommandPostHandlerPipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// The collection of post-handler instances to be executed after the query handler.
    /// </summary>
    private readonly IEnumerable<ICommandPostHandler<TRequest>> _postHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandPostHandlerPipelineBehavior{TRequest,TResponse}"/> class.
    /// </summary>
    /// <param name="postHandlers">The collection of post-handler instances.</param>
    public CommandPostHandlerPipelineBehavior(IEnumerable<ICommandPostHandler<TRequest>> postHandlers)
        => _postHandlers = postHandlers;

    /// <summary>
    /// Handles the query by invoking the next delegate in the pipeline and then executing all post-handlers.
    /// </summary>
    /// <param name="message">The query request message.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The response from the next delegate in the pipeline.</returns>
    public async Task<TResponse> HandleAsync(TRequest message, RequestPipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var response = await next(cancellationToken).ConfigureAwait(false);

        foreach (var postHandler in _postHandlers)
        {
            await postHandler.HandleAsync(message, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }
}
