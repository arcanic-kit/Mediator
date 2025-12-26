using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Event.Abstractions.Pipeline;

namespace Arcanic.Mediator.Event.Pipeline;

/// <summary>
/// Pipeline behavior that executes all registered <see cref="IEventPostHandler{TEvent}"/>
/// instances after the main event handler has processed the event.
/// </summary>
/// <typeparam name="TEvent">
/// The type of the event being handled. Must implement <see cref="IEvent"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the event handler.
/// </typeparam>
public class EventPostHandlerPipelineBehavior<TEvent, TResponse> : IEventPipelineBehavior<TEvent, TResponse>
    where TEvent : IEvent
{
    /// <summary>
    /// Collection of post-handler instances to be executed after the main event handler.
    /// </summary>
    private readonly IEnumerable<IEventPostHandler<TEvent>> _postHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventPostHandlerPipelineBehavior{TEvent, TResponse}"/> class.
    /// </summary>
    /// <param name="postHandlers">
    /// The collection of post-handler instances to execute.
    /// </param>
    public EventPostHandlerPipelineBehavior(IEnumerable<IEventPostHandler<TEvent>> postHandlers)
        => _postHandlers = postHandlers;

    /// <summary>
    /// Invokes the main event handler, then executes all post-handlers for the event.
    /// </summary>
    /// <param name="message">The event message to process.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of type <typeparamref name="TResponse"/>.
    /// </returns>
    public async Task<TResponse> HandleAsync(TEvent message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var response = await next(cancellationToken).ConfigureAwait(false);

        foreach (var postHandler in _postHandlers)
        {
            await postHandler.HandleAsync(message, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }
}
