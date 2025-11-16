using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Sample.WebApi.Domain.Events;

public class ProductCreatedEvent: IEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
