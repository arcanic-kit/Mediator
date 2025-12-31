using Arcanic.Mediator.Command.Abstractions.Handler;

namespace Arcanic.Mediator.Command.Benchmarks.UpdateUser;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    public Task HandleAsync(UpdateUserCommand request, CancellationToken cancellationToken = default)
    {
        // Simulate user update
        return Task.CompletedTask;
    }
}
