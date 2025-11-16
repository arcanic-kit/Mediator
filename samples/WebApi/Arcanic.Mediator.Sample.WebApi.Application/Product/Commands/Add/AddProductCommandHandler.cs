using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Sample.WebApi.Domain.Events;

namespace Arcanic.Mediator.Samples.WebApi.Application.Product.Commands.Add;

public class AddProductCommandHandler(IEventMediator eventMediator) : ICommandHandler<AddProductCommand, int>
{
    public async Task<int> HandleAsync(AddProductCommand request, CancellationToken cancellationToken = default)
    {
        await eventMediator.PublishAsync(new ProductCreatedEvent
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price
        }, cancellationToken);

        return 1;
    }
}
