using Arcanic.Mediator.Event.Abstractions.Handler;
using CleanArchitecture.Domain.Products.Events;

namespace CleanArchitecture.Application.Product.Events.ProductCreated.PreHandlers;

/// <summary>
/// Pre-handler for validation ProductCreatedEvent before the main event handlers execute.
/// This demonstrates cross-cutting concerns like data validation that should run before event processing.
/// </summary>
public class ProductCreatedEventValidationPreHandler : IEventPreHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Example validation logic
        await Task.Run(() => 
        {
            if (@event.Id == Guid.Empty)
            {
                Console.WriteLine($"[EVENT PRE-HANDLER] Validation: Invalid Product ID: {@event.Id}");
                throw new ArgumentException($"Product ID cannot be empty. Received: {@event.Id}", nameof(@event.Id));
            }

            if (string.IsNullOrWhiteSpace(@event.Name))
            {
                Console.WriteLine($"[EVENT PRE-HANDLER] Validation: Invalid Product Name: {@event.Name}");
                throw new ArgumentException($"Product Name cannot be null or empty. Received: {@event.Name}", nameof(@event.Name));
            }

            if (@event.Price < 0)
            {
                Console.WriteLine($"[EVENT PRE-HANDLER] Validation: Invalid Product Price: {@event.Price}");
                throw new ArgumentException($"Product Price cannot be negative. Received: {@event.Price}", nameof(@event.Price));
            }
            
            Console.WriteLine($"[EVENT PRE-HANDLER] Validation: ProductCreatedEvent validation passed for ID: {@event.Id}");
        }, cancellationToken);
    }
}