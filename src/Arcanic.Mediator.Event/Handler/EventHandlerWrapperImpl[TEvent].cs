using System.Windows.Input;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Event.Handler;

public class EventHandlerWrapperImpl<TEvent> : EventHandlerWrapper
    where TEvent : IEvent
{
    public override async Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await Handle((ICommand) request, serviceProvider, cancellationToken).ConfigureAwait(false);
    
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
            .GetServices<IRequestPipelineBehavior<TEvent, Unit>>()
            .Reverse()
            .Aggregate((RequestPipelineDelegate<Unit>) Handler,
                (next, pipeline) => (t) => pipeline.HandleAsync((TEvent) request, next, t == default ? cancellationToken : t))();
    }
}
