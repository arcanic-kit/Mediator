using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Command.Tests.Utils;

namespace Arcanic.Mediator.Command.Tests.Data.Commands.Simple;

public class SimpleCommandHandler(ExecutedTypeTracker executedTypeTracker) : ICommandHandler<SimpleCommand, SimpleCommandResponse>
{
    public Task<SimpleCommandResponse> HandleAsync(SimpleCommand command, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(GetType());
        
        return Task.FromResult(new SimpleCommandResponse
        {
            Result = command.Value,
            Message = $"Processed {command.Value}"
        });
    }
}