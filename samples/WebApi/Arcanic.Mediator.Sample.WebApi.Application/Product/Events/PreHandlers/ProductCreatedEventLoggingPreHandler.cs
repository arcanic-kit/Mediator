using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Sample.WebApi.Domain.Events;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Events.PreHandlers;

/// <summary>
/// Pre-handler for logging ProductCreatedEvent before the main event handlers execute.
/// This demonstrates cross-cutting concerns like auditing/logging that should run before event processing.
/// </summary>
public class ProductCreatedEventLoggingPreHandler : IEventPreHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Example logging logic
        await Task.Run(() => 
        {
            Console.WriteLine($"[EVENT PRE-HANDLER] Logging: ProductCreatedEvent received - ID: {@event.Id}, Name: {@event.Name}, Price: {@event.Price:C}");
        }, cancellationToken);
    }
}