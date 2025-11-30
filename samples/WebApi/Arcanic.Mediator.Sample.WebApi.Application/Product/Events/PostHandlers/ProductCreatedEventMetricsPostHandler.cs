using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Sample.WebApi.Domain.Events;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Events.PostHandlers;

/// <summary>
/// Post-handler for metrics collection after ProductCreatedEvent processing completes.
/// This demonstrates cross-cutting concerns like performance monitoring that should run after event processing.
/// </summary>
public class ProductCreatedEventMetricsPostHandler : IEventPostHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Example metrics logic
        await Task.Run(() => 
        {
            Console.WriteLine($"[EVENT POST-HANDLER] Metrics: ProductCreatedEvent for ID {@event.Id} processed - Recording performance metrics");
        }, cancellationToken);
    }
}