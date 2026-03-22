using Arcanic.Mediator.Command.Abstractions.Handler;
using CleanArchitecture.Domain.Products;

namespace CleanArchitecture.Application.Product.Commands.Add.PreHandlers;

/// <summary>
/// Pre-handler for validating AddProductCommand before the main handler executes.
/// This demonstrates cross-cutting concerns like validation that should run before the main business logic.
/// </summary>
public class AddProductCommandValidationPreHandler(IProductRepository productRepository) : ICommandPreHandler<AddProductCommand>
{
    public async Task HandleAsync(AddProductCommand command, CancellationToken cancellationToken = default)
    {
        // Example validation logic
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException("Product name cannot be empty", nameof(command.Name));
        }

        if (command.Price <= 0)
        {
            throw new ArgumentException("Product price must be greater than zero", nameof(command.Price));
        }

        // Check for duplicate names during add operation
        var existingProduct = await productRepository.GetByNameAsync(command.Name);
        if (existingProduct != null)
        {
            throw new InvalidOperationException($"Product with name '{command.Name}' already exists. Cannot add duplicate product names.");
        }
        
        Console.WriteLine($"[PRE-HANDLER] Validation passed for new product: {command.Name}");
    }
}