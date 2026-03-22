using Arcanic.Mediator.Command.Abstractions.Handler;

namespace CleanArchitecture.Application.Product.Commands.Update.PostHandlers;

/// <summary>
/// Post-handler for sending notifications after UpdateProductCommand completes successfully.
/// This demonstrates cross-cutting concerns like notifications that should run after the main business logic.
/// </summary>
public class UpdateProductCommandNotificationPostHandler : ICommandPostHandler<UpdateProductCommand>
{
    public async Task HandleAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        // Example notification logic for updated products
        await Task.Run(() => 
        {
            Console.WriteLine($"[POST-HANDLER] Notification: Product '{command.Name}' (ID: {command.Id}) has been updated and stakeholders notified");
        }, cancellationToken);
    }
}