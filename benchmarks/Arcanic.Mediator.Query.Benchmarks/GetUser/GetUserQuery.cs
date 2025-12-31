using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Query.Benchmarks.GetUser;

public class GetUserQuery : IQuery<GetUserQueryResponse>
{
    public int Id { get; init; }
}
