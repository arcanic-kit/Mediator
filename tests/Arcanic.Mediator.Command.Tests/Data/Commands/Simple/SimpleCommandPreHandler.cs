using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Command.Tests.Utils;

namespace Arcanic.Mediator.Command.Tests.Data.Commands.Simple;

public class SimpleCommandPreHandler(ExecutedTypeTracker executedTypeTracker) : ICommandPreHandler<SimpleCommand>
{
    public Task HandleAsync(SimpleCommand command, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(GetType());
        return Task.CompletedTask;
    }
}