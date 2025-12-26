using Arcanic.Mediator.Command.Abstractions.Handler;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Commands.UpdatePrice;

public class UpdateProductPriceCommandHandler : ICommandHandler<UpdateProductPriceCommand>
{
    public async Task HandleAsync(UpdateProductPriceCommand message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[HANDLER] Update Product Price: {message.Id}, New Price: {message.Price}");
    }
}
