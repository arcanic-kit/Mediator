using Arcanic.Mediator.Command.Abstractions.Handler;

namespace Arcanic.Mediator.Command.Tests.Data.Commands.Cancellable;

public class CancellableCommandHandler : ICommandHandler<CancellableCommand, CancellableCommandResponse>
{
    public async Task<CancellableCommandResponse> HandleAsync(CancellableCommand command, CancellationToken cancellationToken = default)
    {
        await Task.Delay(command.DelayMilliseconds, cancellationToken);
        return new CancellableCommandResponse
        {
            Message = "Command completed"
        };
    }
}