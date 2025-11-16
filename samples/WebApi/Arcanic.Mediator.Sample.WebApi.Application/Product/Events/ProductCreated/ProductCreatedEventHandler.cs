using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Samples.WebApi.Application.Product.Events.Created;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Events.ProductCreated;

public class ProductCreatedEventHandler : IEventHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent request, CancellationToken cancellationToken = default)
    {
        var message = $"Product created: Id={request.Id}, Name={request.Name}, Price={request.Price}";
        await Task.Run(() => Console.WriteLine(message), cancellationToken);    
    }
}
