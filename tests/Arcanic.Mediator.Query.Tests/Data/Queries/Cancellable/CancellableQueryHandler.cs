using Arcanic.Mediator.Query.Abstractions.Handler;

namespace Arcanic.Mediator.Query.Tests.Data.Queries.Cancellable;

public class CancellableQueryHandler : IQueryHandler<CancellableQuery, CancellableQueryResponse>
{
    public async Task<CancellableQueryResponse> HandleAsync(CancellableQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.Delay(query.DelayMilliseconds, cancellationToken);
            return new CancellableQueryResponse 
            { 
                Completed = true, 
                Message = "Query completed successfully" 
            };
        }
        catch (OperationCanceledException)
        {
            throw; // Re-throw cancellation exceptions
        }
    }
}