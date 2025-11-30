using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Samples.WebApi.Application.Product.Commands.Add;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Commands.Add.PostHandlers;

/// <summary>
/// Post-handler for sending notifications after AddProductCommand completes successfully.
/// This demonstrates cross-cutting concerns like notifications that should run after the main business logic.
/// </summary>
public class AddProductCommandNotificationPostHandler : ICommandPostHandler<AddProductCommand, int>
{
    public async Task HandleAsync(AddProductCommand command, int result, CancellationToken cancellationToken = default)
    {
        // Example notification logic
        await Task.Run(() => 
        {
            Console.WriteLine($"[POST-HANDLER] Notification: New product '{command.Name}' (ID: {result}) has been created and stakeholders notified");
        }, cancellationToken);
    }
}