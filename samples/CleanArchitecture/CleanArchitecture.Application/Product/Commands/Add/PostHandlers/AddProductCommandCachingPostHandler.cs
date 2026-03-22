using Arcanic.Mediator.Command.Abstractions.Handler;

namespace CleanArchitecture.Application.Product.Commands.Add.PostHandlers;

/// <summary>
/// Post-handler for caching operations after AddProductCommand completes successfully.
/// This demonstrates cross-cutting concerns like caching that should run after the main business logic.
/// </summary>
public class AddProductCommandCachingPostHandler : ICommandPostHandler<AddProductCommand>
{
    public async Task HandleAsync(AddProductCommand command, CancellationToken cancellationToken = default)
    {
        // Example caching logic for newly added products
        await Task.Run(() => 
        {
            Console.WriteLine($"[POST-HANDLER] Caching: New product '{command.Name}' cached successfully");
        }, cancellationToken);
    }
}