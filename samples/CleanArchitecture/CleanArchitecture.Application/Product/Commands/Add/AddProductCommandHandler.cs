using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Event.Abstractions;
using CleanArchitecture.Domain.Products;
using CleanArchitecture.Domain.Products.Events;

namespace CleanArchitecture.Application.Product.Commands.Add;

public class AddProductCommandHandler(IProductRepository productRepository, IPublisher publisher) 
    : ICommandHandler<AddProductCommand, int>
{
    public async Task<int> HandleAsync(AddProductCommand request, CancellationToken cancellationToken = default)
    {
        // Check if product with same name already exists
        var existingProduct = await productRepository.GetByNameAsync(request.Name);
        if (existingProduct != null)
        {
            throw new InvalidOperationException($"Product with name '{request.Name}' already exists. Use UpdateProductCommand to update existing products.");
        }

        // Create a new product entity
        var product = new Domain.Products.Product(request.Name, request.Price);

        // Add the product using the repository
        var addedProduct = await productRepository.AddAsync(product);

        // Publish product created event
        await publisher.PublishAsync(new ProductCreatedEvent
        {
            Name = addedProduct.Name,
            Price = addedProduct.Price
        }, cancellationToken);

        // Return the generated product ID
        return addedProduct.Id;
    }
}