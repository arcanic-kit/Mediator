using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Sample.WebApi.Domain.Events;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Events;

public class ConsoleProductCreatedEventHandler : IEventHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent request, CancellationToken cancellationToken = default)
    {
        var message = $"Product created: Id={request.Id}, Name={request.Name}, Price={request.Price}";
        await Task.Run(() => Console.WriteLine(message), cancellationToken);    
    }
}
