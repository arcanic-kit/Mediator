using System.Windows.Input;
using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Handler;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Event.Handler;

/// <summary>
/// Concrete implementation of <see cref="EventHandlerWrapper"/> for a specific event type.
/// Handles the execution of event handlers and applies pipeline behaviors for the given event.
/// </summary>
/// <typeparam name="TEvent">The type of event to handle, which must implement <see cref="IEvent"/>.</typeparam>
public class EventHandlerWrapperImpl<TEvent> : EventHandlerWrapper
    where TEvent : IEvent
{
    /// <summary>
    /// Handles the specified request by casting it to <see cref="ICommand"/> and delegating to the strongly-typed handler.
    /// </summary>
    /// <param name="request">The event request object to handle.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing an optional result object.
    /// </returns>
    public override async Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await Handle((ICommand) request, serviceProvider, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Handles the specified event by invoking the registered event handler and applying all pipeline behaviors.
    /// </summary>
    /// <param name="request">The event to handle.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a <see cref="Unit"/> result.
    /// </returns>
    public override Task<Unit> Handle(IEvent request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        // Defines the core handler delegate that invokes the command handler.
        async Task<Unit> Handler(CancellationToken t = default)
        {
            await serviceProvider.GetRequiredService<IEventHandler<TEvent>>()
                .HandleAsync((TEvent) request, t == default ? cancellationToken : t);

            return Unit.Value;
        }

        // Applies all pipeline behaviors in reverse order, wrapping the handler.
        return serviceProvider
            .GetServices<IPipelineBehavior<TEvent, Unit>>()
            .Reverse()
            .Aggregate((PipelineDelegate<Unit>) Handler,
                (next, pipeline) => (t) => pipeline.HandleAsync((TEvent) request, next, t == default ? cancellationToken : t))();
    }
}
