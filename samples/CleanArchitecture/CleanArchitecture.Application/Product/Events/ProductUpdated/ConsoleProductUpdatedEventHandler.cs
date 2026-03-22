using Arcanic.Mediator.Event.Abstractions.Handler;
using CleanArchitecture.Domain.Products.Events;

namespace CleanArchitecture.Application.Product.Events.ProductUpdated;

/// <summary>
/// Event handler for ProductUpdatedEvent that handles logging and notifications when a product is updated.
/// </summary>
public class ConsoleProductUpdatedEventHandler : IEventHandler<ProductUpdatedEvent>
{
    public async Task HandleAsync(ProductUpdatedEvent request, CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            if (request.OldPrice.HasValue && request.OldPrice != request.Price)
            {
                Console.WriteLine($"[EVENT HANDLER] Product '{request.Name}' (ID: {request.ProductId}) price updated from {request.OldPrice:C} to {request.Price:C}");
            }
            else
            {
                Console.WriteLine($"[EVENT HANDLER] Product '{request.Name}' (ID: {request.ProductId}) was updated");
            }
        }, cancellationToken);
    }
}