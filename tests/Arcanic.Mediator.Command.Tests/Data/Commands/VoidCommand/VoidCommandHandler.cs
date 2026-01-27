using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Command.Tests.Utils;

namespace Arcanic.Mediator.Command.Tests.Data.Commands.VoidCommand;

public class VoidCommandHandler(ExecutedTypeTracker executedTypeTracker) : ICommandHandler<VoidCommand>
{
    public Task HandleAsync(VoidCommand command, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(GetType());
        return Task.CompletedTask;
    }
}