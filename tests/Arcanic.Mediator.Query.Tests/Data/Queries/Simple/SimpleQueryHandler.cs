using Arcanic.Mediator.Query.Abstractions.Handler;
using Arcanic.Mediator.Query.Tests.Utils;

namespace Arcanic.Mediator.Query.Tests.Data.Queries.Simple;

public class SimpleQueryHandler(ExecutedTypeTracker executedTypeTracker) : IQueryHandler<SimpleQuery, SimpleQueryResponse>
{
    public Task<SimpleQueryResponse> HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(GetType());
        
        return Task.FromResult(new SimpleQueryResponse
        {
            Result = query.Value,
            Message = $"Processed {query.Value}"
        });
    }
}