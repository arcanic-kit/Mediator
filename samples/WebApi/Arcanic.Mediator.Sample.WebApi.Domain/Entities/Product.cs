namespace Arcanic.Mediator.Sample.WebApi.Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public DateTime CreateAt { get; private set; } 
    public string Name { get; private set; } 
    public string? Description { get; set; }
    public decimal Price { get; private set; }

    public Product(int id, string name, decimal price)
    {
        Id = id;
        CreateAt = DateTime.UtcNow;
        Name = name;
        Price = price;
    }
}
