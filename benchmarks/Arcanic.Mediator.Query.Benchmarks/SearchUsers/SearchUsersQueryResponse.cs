using System.Collections.Generic;

namespace Arcanic.Mediator.Query.Benchmarks.SearchUsers;

public class SearchUsersQueryResponse
{
    public IEnumerable<Dtos.User> Users { get; init; } = [];
    public int TotalCount { get; init; }
}