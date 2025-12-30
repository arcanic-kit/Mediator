using Arcanic.Mediator.Event.Abstractions;

namespace CleanArchitecture.Domain.Products.Events;

public class ProductCreatedEvent: IEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
