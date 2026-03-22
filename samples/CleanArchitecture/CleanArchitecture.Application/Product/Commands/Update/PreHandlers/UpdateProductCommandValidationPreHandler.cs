using Arcanic.Mediator.Command.Abstractions.Handler;
using CleanArchitecture.Domain.Products;

namespace CleanArchitecture.Application.Product.Commands.Update.PreHandlers;

/// <summary>
/// Pre-handler for validating UpdateProductCommand before the main handler executes.
/// This demonstrates cross-cutting concerns like validation that should run before the main business logic.
/// </summary>
public class UpdateProductCommandValidationPreHandler(IProductRepository productRepository) : ICommandPreHandler<UpdateProductCommand>
{
    public async Task HandleAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        // Basic validation
        if (command.Id <= 0)
        {
            throw new ArgumentException("Product ID must be greater than zero", nameof(command.Id));
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException("Product name cannot be empty", nameof(command.Name));
        }

        if (command.Price <= 0)
        {
            throw new ArgumentException("Product price must be greater than zero", nameof(command.Price));
        }

        // Check if the product exists
        var existingProduct = await productRepository.GetByIdAsync(command.Id);
        if (existingProduct == null)
        {
            throw new InvalidOperationException($"Product with ID {command.Id} does not exist and cannot be updated");
        }

        // Check for name conflicts with other products
        var productWithSameName = await productRepository.GetByNameAsync(command.Name);
        if (productWithSameName != null && productWithSameName.Id != command.Id)
        {
            throw new InvalidOperationException($"Another product with name '{command.Name}' already exists (ID: {productWithSameName.Id}). Product names must be unique.");
        }
        
        Console.WriteLine($"[PRE-HANDLER] Validation passed for product update: {command.Name} (ID: {command.Id})");
    }
}