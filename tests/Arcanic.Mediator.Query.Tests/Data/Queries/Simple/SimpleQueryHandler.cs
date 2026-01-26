using Arcanic.Mediator.Query.Abstractions.Handler;

namespace Arcanic.Mediator.Query.Tests.Data.Queries.Simple;

public class SimpleQueryHandler : IQueryHandler<SimpleQuery, SimpleQueryResponse>
{
    public Task<SimpleQueryResponse> HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
    {
        query.ExecutedTypes.Add(GetType());
        
        return Task.FromResult(new SimpleQueryResponse
        {
            Result = 100,
            Message = $"Processed {100}"
        });
    }
}