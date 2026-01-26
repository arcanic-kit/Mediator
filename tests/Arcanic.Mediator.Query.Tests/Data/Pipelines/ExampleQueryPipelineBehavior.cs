using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Pipeline;

namespace Arcanic.Mediator.Query.Tests.Data.Pipelines;

public class ExampleQueryPipelineBehavior<TQuery, TResponse> : IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public bool Executed { get; private set; }
    
    public async Task<TResponse> HandleAsync(TQuery query, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        Executed = true;
        return await next(cancellationToken);
    }
}
