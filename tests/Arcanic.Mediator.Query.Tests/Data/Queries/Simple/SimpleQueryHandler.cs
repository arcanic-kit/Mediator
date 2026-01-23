using Arcanic.Mediator.Query.Abstractions.Handler;

namespace Arcanic.Mediator.Query.Tests.Data.Queries.Simple;

public class SimpleQueryHandler : IQueryHandler<SimpleQuery, SimpleQueryResponse>
{
    public Task<SimpleQueryResponse> HandleAsync(SimpleQuery request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new SimpleQueryResponse
        {
            Result = request.Value * 2,
            Message = $"Processed {request.Value}"
        });
    }
}