using Arcanic.Mediator.Command.Abstractions.Handler;

namespace CleanArchitecture.Application.Product.Commands.Update.PreHandlers;

/// <summary>
/// Pre-handler for logging UpdateProductCommand before the main handler executes.
/// This demonstrates cross-cutting concerns like auditing/logging that should run before the main business logic.
/// </summary>
public class UpdateProductCommandLoggingPreHandler : ICommandPreHandler<UpdateProductCommand>
{
    public async Task HandleAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        // Example logging logic
        await Task.Run(() => 
        {
            Console.WriteLine($"[PRE-HANDLER] Logging: UpdateProductCommand received - ID: {command.Id}, Name: {command.Name}, Price: {command.Price:C}");
        }, cancellationToken);
    }
}