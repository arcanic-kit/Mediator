using Arcanic.Mediator.Command.Abstractions;

namespace CleanArchitecture.Application.Product.Commands.Update;

public class UpdateProductCommand : ICommand<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}