using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Benchmarks.Commands.DeleteUser;

public class DeleteUserCommand : ICommand
{
    public int Id { get; init; }
    public string Reason { get; init; } = string.Empty;
}
