namespace Arcanic.Mediator.Benchmarks.Queries.SearchUsers;

public class SearchUsersQueryResponse
{
    public IEnumerable<Dtos.User> Users { get; init; } = [];
    public int TotalCount { get; init; }
}