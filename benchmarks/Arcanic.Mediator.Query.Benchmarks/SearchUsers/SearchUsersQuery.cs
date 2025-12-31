using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Query.Benchmarks.SearchUsers;

public class SearchUsersQuery : IQuery<SearchUsersQueryResponse>
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
