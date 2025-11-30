using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Samples.WebApi.Application.Product.Commands.Add;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Commands.Add.PreHandlers;

/// <summary>
/// Pre-handler for validating AddProductCommand before the main handler executes.
/// This demonstrates cross-cutting concerns like validation that should run before the main business logic.
/// </summary>
public class AddProductCommandValidationPreHandler : ICommandPreHandler<AddProductCommand>
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

        // Simulate async validation (e.g., checking if product name already exists)
        await Task.Delay(10, cancellationToken);
        
        Console.WriteLine($"[PRE-HANDLER] Validation passed for product: {command.Name}");
    }
}