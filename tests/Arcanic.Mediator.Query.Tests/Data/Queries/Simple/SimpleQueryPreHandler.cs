using Arcanic.Mediator.Query.Abstractions.Handler;
using Arcanic.Mediator.Query.Tests.Utils;

namespace Arcanic.Mediator.Query.Tests.Data.Queries.Simple;

public class SimpleQueryPreHandler(ExecutedTypeTracker executedTypeTracker): IQueryPreHandler<SimpleQuery>
{
    public bool WasExecuted { get; private set; }
    public SimpleQuery? ReceivedQuery { get; private set; }

    public Task HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
    {
        executedTypeTracker.ExecutedTypes.Add(GetType());
        
        WasExecuted = true;
        ReceivedQuery = query;
        
        return Task.CompletedTask;
    }
}