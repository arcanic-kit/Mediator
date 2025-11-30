using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Samples.WebApi.Application.Product.Commands.Add;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Commands.Add.PreHandlers;

/// <summary>
/// Pre-handler for logging AddProductCommand before the main handler executes.
/// This demonstrates cross-cutting concerns like auditing/logging that should run before the main business logic.
/// </summary>
public class AddProductCommandLoggingPreHandler : ICommandPreHandler<AddProductCommand>
{
    public async Task HandleAsync(AddProductCommand command, CancellationToken cancellationToken = default)
    {
        // Example logging logic
        await Task.Run(() => 
        {
            Console.WriteLine($"[PRE-HANDLER] Logging: AddProductCommand received - Name: {command.Name}, Price: {command.Price:C}");
        }, cancellationToken);
    }
}