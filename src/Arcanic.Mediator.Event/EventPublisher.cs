using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Mediator;
using Arcanic.Mediator.Messaging.Mediator.Strategies;

namespace Arcanic.Mediator.Event;

/// <summary>
/// Provides event mediation capabilities by coordinating the publishing and handling of events
/// through the underlying message mediator framework. This class serves as a specialized facade
/// for event-driven communication patterns within the application.
/// </summary>
public class EventPublisher : IEventPublisher
{
    private readonly IMessageMediator _messageMediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventPublisher"/> class with the specified message mediator.
    /// </summary>
    /// <param name="messageMediator">The message mediator instance used for coordinating event processing.
    /// This parameter cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageMediator"/> is null.</exception>
    public EventPublisher(IMessageMediator messageMediator)
    {
        _messageMediator = messageMediator ?? throw new ArgumentNullException(nameof(messageMediator));
    }

    /// <summary>
    /// Asynchronously publishes an event to all registered handlers using a multiple handler strategy.
    /// The event will be processed by all handlers that can handle the specified event type,
    /// allowing for fan-out event distribution patterns.
    /// </summary>
    /// <param name="request">The event to publish. This parameter cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the publishing operation.
    /// The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous publishing operation. The task completes when
    /// all registered event handlers have finished processing the event.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    public async Task PublishAsync(IEvent request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var strategy = new MessageMediatorPipelineEventHandlerStrategy<IEvent>();
        var options = new MessageMediatorOptions<IEvent, Task>()
        {
            Strategy = strategy,
            CancellationToken = cancellationToken,
        };

        await _messageMediator.Mediate(request, options);
    }
}
