using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Query.Tests.Data.Queries.UnhandledQuery;

public class UnhandledQuery : IQuery<UnhandledQueryResponse>
{
    public string Message { get; set; } = string.Empty;
}