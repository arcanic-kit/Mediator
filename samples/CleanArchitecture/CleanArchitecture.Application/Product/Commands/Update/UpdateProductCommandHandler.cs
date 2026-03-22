using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Event.Abstractions;
using CleanArchitecture.Domain.Products;
using CleanArchitecture.Domain.Products.Events;

namespace CleanArchitecture.Application.Product.Commands.Update;

public class UpdateProductCommandHandler(IProductRepository productRepository, IPublisher publisher) 
    : ICommandHandler<UpdateProductCommand, int>
{
    public async Task<int> HandleAsync(UpdateProductCommand request, CancellationToken cancellationToken = default)
    {
        // Get the existing product by ID
        var existingProduct = await productRepository.GetByIdAsync(request.Id);
        if (existingProduct == null)
        {
            throw new InvalidOperationException($"Product with ID '{request.Id}' not found. Cannot update non-existent product.");
        }

        // Check if another product with the same name exists (but different ID)
        var productWithSameName = await productRepository.GetByNameAsync(request.Name);
        if (productWithSameName != null && productWithSameName.Id != request.Id)
        {
            throw new InvalidOperationException($"Another product with name '{request.Name}' already exists (ID: {productWithSameName.Id}). Product names must be unique.");
        }

        // Store old price for event
        var oldPrice = existingProduct.Price;

        // Update the product
        existingProduct.Update(request.Name, request.Price);
        var updatedProduct = await productRepository.UpdateAsync(existingProduct);

        // Publish product updated event
        await publisher.PublishAsync(new ProductUpdatedEvent
        {
            ProductId = updatedProduct.Id,
            Name = updatedProduct.Name,
            Price = updatedProduct.Price,
            OldPrice = oldPrice
        }, cancellationToken);

        // Return the product ID
        return updatedProduct.Id;
    }
}