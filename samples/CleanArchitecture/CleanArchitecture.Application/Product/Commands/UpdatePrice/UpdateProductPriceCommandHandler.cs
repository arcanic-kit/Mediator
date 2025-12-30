using Arcanic.Mediator.Command.Abstractions.Handler;

namespace CleanArchitecture.Application.Product.Commands.UpdatePrice;

public class UpdateProductPriceCommandHandler : ICommandHandler<UpdateProductPriceCommand>
{
    public async Task HandleAsync(UpdateProductPriceCommand message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[HANDLER] Update Product Price: {message.Id}, New Price: {message.Price}");
    }
}
