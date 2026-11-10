using Arcanic.Mediator.Pipeline;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Event.Abstractions.Pipeline;

namespace Arcanic.Mediator.Event.Pipeline;

/// <summary>
/// Pipeline behavior that runs every registered <see cref="IEventPreHandler{T}"/> for the pre-handler stage of a event.
/// </summary>
/// <typeparam name="TEvent">The event type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class EventPreHandlerPipelineBehavior<TEvent, TResponse>
    : PreHandlerPipelineBehaviorBase<TEvent, TResponse, IEventPreHandler<TEvent>>, IEventPipelineBehavior<TEvent, TResponse>
    where TEvent : IEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventPreHandlerPipelineBehavior{TEvent, TResponse}"/> class.
    /// </summary>
    /// <param name="preHandlers">The pre-handlers registered for <typeparamref name="TEvent"/>.</param>
    public EventPreHandlerPipelineBehavior(IEnumerable<IEventPreHandler<TEvent>> preHandlers)
        : base(preHandlers)
    {
    }
}
