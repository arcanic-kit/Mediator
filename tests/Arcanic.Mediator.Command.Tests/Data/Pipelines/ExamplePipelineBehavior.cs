using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Command.Tests.Utils;

namespace Arcanic.Mediator.Command.Tests.Data.Pipelines;

public class ExamplePipelineBehavior<TMessage, TResponse>(ExecutedTypeTracker executedTypeTracker) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    public async Task<TResponse> HandleAsync(TMessage message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(GetType());
        return await next(cancellationToken);
    }
}