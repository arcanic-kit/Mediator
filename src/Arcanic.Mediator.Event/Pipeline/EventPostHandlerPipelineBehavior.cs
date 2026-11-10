using Arcanic.Mediator.Pipeline;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Event.Abstractions.Pipeline;

namespace Arcanic.Mediator.Event.Pipeline;

/// <summary>
/// Pipeline behavior that runs every registered <see cref="IEventPostHandler{T}"/> for the post-handler stage of a event.
/// </summary>
/// <typeparam name="TEvent">The event type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class EventPostHandlerPipelineBehavior<TEvent, TResponse>
    : PostHandlerPipelineBehaviorBase<TEvent, TResponse, IEventPostHandler<TEvent>>, IEventPipelineBehavior<TEvent, TResponse>
    where TEvent : IEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventPostHandlerPipelineBehavior{TEvent, TResponse}"/> class.
    /// </summary>
    /// <param name="postHandlers">The post-handlers registered for <typeparamref name="TEvent"/>.</param>
    public EventPostHandlerPipelineBehavior(IEnumerable<IEventPostHandler<TEvent>> postHandlers)
        : base(postHandlers)
    {
    }
}
