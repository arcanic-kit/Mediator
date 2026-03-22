using Arcanic.Mediator.Event.Abstractions;

namespace CleanArchitecture.Domain.Products.Events;

public class ProductUpdatedEvent : IEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
}