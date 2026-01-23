using Arcanic.Mediator.Query.Abstractions.Handler;

namespace Arcanic.Mediator.Query.Tests.Data.Queries.Simple;

public class SimpleQueryPreHandler: IQueryPreHandler<SimpleQuery>
{
    public bool WasExecuted { get; private set; }
    public SimpleQuery? ReceivedQuery { get; private set; }

    public Task HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
    {
        WasExecuted = true;
        ReceivedQuery = query;
        return Task.CompletedTask;
    }
}