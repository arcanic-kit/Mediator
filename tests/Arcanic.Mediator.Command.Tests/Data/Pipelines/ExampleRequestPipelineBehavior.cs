using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Request.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Tests.Utils;

namespace Arcanic.Mediator.Command.Tests.Data.Pipelines;

public class ExampleRequestPipelineBehavior<TRequest, TResponse>(ExecutedTypeTracker executedTypeTracker) : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest
{
    public async Task<TResponse> HandleAsync(TRequest request, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(GetType());
        return await next(cancellationToken);
    }
}