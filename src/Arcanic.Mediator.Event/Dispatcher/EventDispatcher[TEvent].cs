using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Event.Abstractions.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Event.Dispatcher;

/// <summary>
/// Concrete implementation of event dispatcher for a specific event type.
/// Handles the dispatch of the appropriate event handler and applies any registered pipeline behaviors.
/// </summary>
/// <typeparam name="TEvent">The type of event to dispatch.</typeparam>
public class EventDispatcher<TEvent> : EventDispatcherBase
    where TEvent : IEvent
{
    /// <summary>
    /// Dispatches the specified event using the provided service provider and cancellation token.
    /// Casts the event to the appropriate type and delegates to the strongly-typed dispatcher.
    /// </summary>
    /// <param name="event">The event to dispatch (as an object).</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with an optional result object.
    /// </returns>
    public override async Task<object?> DispatchAsync(object @event, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await DispatchAsync((IEvent) @event, serviceProvider, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Dispatches the specified event using the provided service provider and cancellation token.
    /// Resolves the event handler and applies all registered pipeline behaviors in reverse order.
    /// </summary>
    /// <param name="event">The event to dispatch.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of type <see cref="Unit"/>.
    /// </returns>
    private async Task<Unit> DispatchAsync(IEvent @event, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        // Defines the core handler delegate that invokes the event handler.
        async Task<Unit> Handler(CancellationToken t = default)
        {
            await serviceProvider.GetRequiredService<IEventHandler<TEvent>>()
                .HandleAsync((TEvent) @event, t == default ? cancellationToken : t);

            return Unit.Value;
        }

        var allPipelineBehaviors = serviceProvider
            .GetServices<IEventPipelineBehavior<TEvent, Unit>>()
            .Concat(serviceProvider.GetServices<IPipelineBehavior<TEvent, Unit>>());

        return await allPipelineBehaviors
            .Aggregate((PipelineDelegate<Unit>) Handler,
                (next, pipeline) => (t) => pipeline.HandleAsync((TEvent) @event, next, t == default ? cancellationToken : t))();
    }
}
