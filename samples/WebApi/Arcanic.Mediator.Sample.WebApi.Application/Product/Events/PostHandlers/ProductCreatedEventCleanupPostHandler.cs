using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Sample.WebApi.Domain.Events;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Events.PostHandlers;

/// <summary>
/// Post-handler for cleanup operations after ProductCreatedEvent processing completes.
/// This demonstrates cross-cutting concerns like resource cleanup that should run after event processing.
/// </summary>
public class ProductCreatedEventCleanupPostHandler : IEventPostHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Example cleanup logic
        await Task.Run(() => 
        {
            Console.WriteLine($"[EVENT POST-HANDLER] Cleanup: ProductCreatedEvent for ID {@event.Id} processed - Performing cleanup operations");
        }, cancellationToken);
    }
}