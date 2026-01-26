using Arcanic.Mediator.Query.Abstractions.Handler;

namespace Arcanic.Mediator.Query.Tests.Data.Queries.Simple;

public class SimpleQueryPostHandler: IQueryPostHandler<SimpleQuery>
{
    public bool WasExecuted { get; private set; }
    public SimpleQuery? ReceivedQuery { get; private set; }

    public Task HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
    {
        query.ExecutedTypes.Add(GetType());
        
        WasExecuted = true;
        ReceivedQuery = query;
        
        return Task.CompletedTask;
    }
}