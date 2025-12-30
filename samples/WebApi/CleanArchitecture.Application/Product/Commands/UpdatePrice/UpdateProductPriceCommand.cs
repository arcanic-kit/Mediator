using Arcanic.Mediator.Command.Abstractions;

namespace CleanArchitecture.Application.Product.Commands.UpdatePrice;

public class UpdateProductPriceCommand: ICommand
{
    public int Id { get; set; }
    public float Price { get; set; }
}
