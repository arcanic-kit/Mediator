using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Handler;

namespace Arcanic.Mediator.Benchmarks.Commands.DeleteUser;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    public Task HandleAsync(DeleteUserCommand request, CancellationToken cancellationToken = default)
    {
        // Simulate user deletion
        return Task.CompletedTask;
    }
}