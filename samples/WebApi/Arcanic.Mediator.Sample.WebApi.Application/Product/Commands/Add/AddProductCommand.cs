using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Commands.Add;

public class AddProductCommand: ICommand<int>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
