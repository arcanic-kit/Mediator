using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Query.Tests.Data.Queries.Cancellable;

public class CancellableQuery : IQuery<CancellableQueryResponse>
{
    public int DelayMilliseconds { get; set; } = 1000;
}