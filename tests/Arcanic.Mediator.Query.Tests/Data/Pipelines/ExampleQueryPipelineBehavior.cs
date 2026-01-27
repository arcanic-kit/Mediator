using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Tests.Utils;

namespace Arcanic.Mediator.Query.Tests.Data.Pipelines;

public class ExampleQueryPipelineBehavior<TQuery, TResponse>(ExecutedTypeTracker executedTypeTracker) : IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> HandleAsync(TQuery query, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(GetType());
        return await next(cancellationToken);
    }
}
