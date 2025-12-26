using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Event.Abstractions.Pipeline;

namespace Arcanic.Mediator.Event.Pipeline;

/// <summary>
/// Pipeline behavior that executes all registered <see cref="IEventPreHandler{TEvent}"/>
/// instances before invoking the next delegate in the event handling pipeline.
/// </summary>
/// <typeparam name="TEvent">
/// The type of the event being handled. Must implement <see cref="IEvent"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the event handler.
/// </typeparam>
public class EventPreHandlerPipelineBehavior<TEvent, TResponse> : IEventPipelineBehavior<TEvent, TResponse>
    where TEvent : IEvent
{
    /// <summary>
    /// Collection of pre-handler instances to be executed before the main event handler.
    /// </summary>
    private readonly IEnumerable<IEventPreHandler<TEvent>> _preHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventPreHandlerPipelineBehavior{TEvent, TResponse}"/> class.
    /// </summary>
    /// <param name="preHandlers">
    /// The collection of pre-handler instances to execute.
    /// </param>
    public EventPreHandlerPipelineBehavior(IEnumerable<IEventPreHandler<TEvent>> preHandlers)
        => _preHandlers = preHandlers;

    /// <summary>
    /// Executes all pre-handlers for the event, then invokes the next delegate in the pipeline.
    /// </summary>
    /// <param name="message">The event message to process.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of type <typeparamref name="TResponse"/>.
    /// </returns>
    public async Task<TResponse> HandleAsync(TEvent message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        foreach (var preHandler in _preHandlers)
        {
            await preHandler.HandleAsync(message, cancellationToken).ConfigureAwait(false);
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
