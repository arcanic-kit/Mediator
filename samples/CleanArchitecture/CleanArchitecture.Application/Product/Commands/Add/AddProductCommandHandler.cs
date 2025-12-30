using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Event.Abstractions;
using CleanArchitecture.Domain.Products.Events;

namespace CleanArchitecture.Application.Product.Commands.Add;

public class AddProductCommandHandler(IEventPublisher eventPublisher) : ICommandHandler<AddProductCommand, int>
{
    public async Task<int> HandleAsync(AddProductCommand request, CancellationToken cancellationToken = default)
    {
        await eventPublisher.PublishAsync(new ProductCreatedEvent
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price
        }, cancellationToken);

        return 1;
    }
}
