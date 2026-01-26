using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Pipeline;

namespace Arcanic.Mediator.Query.Tests.Data.Pipelines;

public class ExamplePipelineBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    public bool Executed { get; private set; }
    
    public async Task<TResponse> HandleAsync(TMessage message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        Executed = true;
        return await next(cancellationToken);
    }
}