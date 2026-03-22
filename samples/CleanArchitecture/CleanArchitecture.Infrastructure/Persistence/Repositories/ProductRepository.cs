using CleanArchitecture.Domain.Products;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

internal class ProductRepository : IProductRepository
{
    private static readonly List<Product> _products = new();
    private static int _nextId = 1;

    public Task<Product?> GetByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task<Product?> GetByNameAsync(string name)
    {
        var product = _products.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(product);
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Product>>(_products.ToList());
    }

    public Task<Product> AddAsync(Product product)
    {
        // Assign a new ID if not set
        if (product.Id == 0)
        {
            product.Id = _nextId++;
        }

        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task<Product> UpdateAsync(Product product)
    {
        var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existingProduct == null)
        {
            throw new InvalidOperationException($"Product with ID {product.Id} not found for update");
        }

        // Remove the old product and add the updated one
        _products.Remove(existingProduct);
        _products.Add(product);

        return Task.FromResult(product);
    }

    public Task DeleteAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _products.Remove(product);
        }
        return Task.CompletedTask;
    }
}
