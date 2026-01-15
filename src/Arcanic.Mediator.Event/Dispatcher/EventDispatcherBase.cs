using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Event.Dispatcher;

/// <summary>
/// Abstract base class for event dispatchers.
/// Provides an interface for dispatching events with dynamic event types and dependency injection.
/// </summary>
public abstract class EventDispatcherBase
{
    /// <summary>
    /// Dispatches the specified event using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="event">The event to dispatch.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with an optional result object.
    /// </returns>
    public abstract Task<object?> DispatchAsync(object @event, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves all pipeline behaviors for the specified event and response types from the service provider.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event that implements <see cref="IEvent"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the event processing.</typeparam>
    /// <param name="serviceProvider">The service provider used to resolve pipeline behavior services.</param>
    /// <returns>
    /// An enumerable collection of pipeline behaviors in reverse order of registration, 
    /// including event-specific and generic pipeline behaviors.
    /// </returns>
    protected IEnumerable<IPipelineBehavior<TEvent, TResponse>> GetAllPipelineBehaviors<TEvent, TResponse>(IServiceProvider serviceProvider)
        where TEvent : IEvent
    {
        // Retrieve event-specific pipeline behaviors in reverse order
        var eventPipelineBehaviors = serviceProvider
            .GetServices<IEventPipelineBehavior<TEvent, TResponse>>()
            .Reverse();
        
        // Retrieve generic pipeline behaviors in reverse order
        var pipelineBehaviors = serviceProvider
            .GetServices<IPipelineBehavior<TEvent, TResponse>>()
            .Reverse();

        // Combine all pipeline behaviors in the correct execution order
        return eventPipelineBehaviors
            .Concat(pipelineBehaviors);
    }
}
