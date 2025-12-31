using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arcanic.Mediator.Query.Abstractions.Handler;

namespace Arcanic.Mediator.Query.Benchmarks.SearchUsers;

public class SearchUsersQueryHandler : IQueryHandler<SearchUsersQuery, SearchUsersQueryResponse>
{
    public Task<SearchUsersQueryResponse> HandleAsync(SearchUsersQuery request, CancellationToken cancellationToken = default)
    {
        // Simulate search operation
        var users = Enumerable.Range(1, request.PageSize)
            .Select(i => new Dtos.User
            {
                Id = i,
                Name = $"User {i}",
                Email = $"user{i}@example.com"
            });

        var result = new SearchUsersQueryResponse
        {
            Users = users,
            TotalCount = 100 // Simulated total count
        };

        return Task.FromResult(result);
    }
}