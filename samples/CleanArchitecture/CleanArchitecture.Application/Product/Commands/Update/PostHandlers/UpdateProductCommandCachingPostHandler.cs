using Arcanic.Mediator.Command.Abstractions.Handler;

namespace CleanArchitecture.Application.Product.Commands.Update.PostHandlers;

/// <summary>
/// Post-handler for caching operations after UpdateProductCommand completes successfully.
/// This demonstrates cross-cutting concerns like caching that should run after the main business logic.
/// </summary>
public class UpdateProductCommandCachingPostHandler : ICommandPostHandler<UpdateProductCommand>
{
    public async Task HandleAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        // Example caching logic for updated products
        await Task.Run(() => 
        {
            Console.WriteLine($"[POST-HANDLER] Caching: Updated product '{command.Name}' (ID: {command.Id}) cached successfully");
        }, cancellationToken);
    }
}