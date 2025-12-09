using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Benchmarks.Commands.UpdateUser;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    public Task HandleAsync(UpdateUserCommand request, CancellationToken cancellationToken = default)
    {
        // Simulate user update
        return Task.CompletedTask;
    }
}
