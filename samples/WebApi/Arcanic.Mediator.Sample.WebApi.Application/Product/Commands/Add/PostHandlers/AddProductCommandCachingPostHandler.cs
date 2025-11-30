using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Samples.WebApi.Application.Product.Commands.Add;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Commands.Add.PostHandlers;

/// <summary>
/// Post-handler for caching operations after AddProductCommand completes successfully.
/// This demonstrates cross-cutting concerns like caching that should run after the main business logic.
/// </summary>
public class AddProductCommandCachingPostHandler : ICommandPostHandler<AddProductCommand, int>
{
    public async Task HandleAsync(AddProductCommand command, int result, CancellationToken cancellationToken = default)
    {
        // Example caching logic
        await Task.Run(() => 
        {
            Console.WriteLine($"[POST-HANDLER] Caching: Product {command.Name} with ID {result} cached successfully");
        }, cancellationToken);
    }
}