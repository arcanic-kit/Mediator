namespace CleanArchitecture.Domain.Products;

public class Product
{
    public int Id { get; set; }
    public DateTime CreateAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public string Name { get; private set; }
    public string? Description { get; set; }
    public decimal Price { get; private set; }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
        CreateAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the product's name and price
    /// </summary>
    public void Update(string name, decimal price)
    {
        Name = name;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }
}
