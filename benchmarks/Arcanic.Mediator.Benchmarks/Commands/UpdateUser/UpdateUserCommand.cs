using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Benchmarks.Commands.UpdateUser;

public class UpdateUserCommand : ICommand
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}