using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Samples.WebApi.Application.Product.Events.Created;

public class ProductCreatedEvent: IEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
