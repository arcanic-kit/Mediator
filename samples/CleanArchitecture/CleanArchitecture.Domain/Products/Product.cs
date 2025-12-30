namespace CleanArchitecture.Domain.Products;

public class Product(int id, string name, decimal price)
{
    public int Id { get; private set; } = id;
    public DateTime CreateAt { get; private set; } = DateTime.UtcNow;
    public string Name { get; private set; } = name;
    public string? Description { get; set; }
    public decimal Price { get; private set; } = price;
}
