using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Query.Tests.Data.Queries.Simple;

public class SimpleQuery : IQuery<SimpleQueryResponse>
{
    public int Value { get; set; }
}