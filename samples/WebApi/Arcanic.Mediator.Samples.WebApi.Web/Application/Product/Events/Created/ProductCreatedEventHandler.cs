using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Samples.WebApi.Application.Product.Events.Created;

public class ProductCreatedEventHandler : IEventHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent request, CancellationToken cancellationToken = default)
    {
        var message = $"Product created: Id={request.Id}, Name={request.Name}, Price={request.Price}";
        await Task.Run(() => Console.WriteLine(message), cancellationToken);    
    }
}
