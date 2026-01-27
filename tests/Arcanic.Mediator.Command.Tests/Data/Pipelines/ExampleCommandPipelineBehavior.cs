using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Tests.Utils;

namespace Arcanic.Mediator.Command.Tests.Data.Pipelines;

public class ExampleCommandPipelineBehavior<TCommand, TResponse>(ExecutedTypeTracker executedTypeTracker) : ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand
{
    public async Task<TResponse> HandleAsync(TCommand command, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(GetType());
        return await next(cancellationToken);
    }
}