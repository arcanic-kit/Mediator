﻿using System.Collections.Concurrent;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Dispatcher;

namespace Arcanic.Mediator.Event;

/// <summary>
/// Provides event mediation capabilities by coordinating the publishing and handling of events
/// through the underlying message mediator framework. This class serves as a specialized facade
/// for event-driven communication patterns within the application.
/// </summary>
public class EventPublisher : IEventPublisher
{
    /// <summary>
    /// The service provider used for dependency injection and handler resolution.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// A thread-safe cache mapping event types to their corresponding dispatcher instances.
    /// This avoids repeated allocations and reflection for each event publication.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, EventDispatcherBase> EventDispatchers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EventPublisher"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving dependencies and handlers.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is null.</exception>
    public EventPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Publishes an event asynchronously, routing it through the appropriate event handler pipeline.
    /// Uses cached dispatcher instances to improve performance.
    /// </summary>
    /// <param name="event">The event to publish.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during event processing.</param>
    /// <returns>A task that represents the asynchronous event publishing operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="event"/> is null.</exception>
    public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var eventType = @event.GetType();

        // Retrieve or create a dispatcher for the event type.
        var dispatcher = EventDispatchers.GetOrAdd(eventType, static requestType =>
        {
            var dispatcherType = typeof(Dispatcher.EventDispatcher<>).MakeGenericType(requestType);
            var dispatcherInstance = Activator.CreateInstance(dispatcherType) ?? throw new InvalidOperationException($"Could not create dispatcher type for {requestType}");
            return (EventDispatcherBase)dispatcherInstance;
        });

        // Delegate the dispatching of the event to the resolved dispatcher.
        await dispatcher.DispatchAsync(@event, _serviceProvider, cancellationToken);
    }
}
