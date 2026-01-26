using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Request.Abstractions.Pipeline;

namespace Arcanic.Mediator.Query.Tests.Data.Pipelines;

public class ExampleRequestPipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest
{
    public bool Executed { get; private set; }
    
    public async Task<TResponse> HandleAsync(TRequest request, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        Executed = true;
        return await next(cancellationToken);
    }
}
