using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Benchmarks.Commands.CreateUser;

public class CreateUserCommand : ICommand<CreateUserCommandResponse>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
