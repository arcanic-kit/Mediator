using Arcanic.Mediator.Benchmarks.Models;
using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Benchmarks.Arcanic.Queries;

/// <summary>
/// Arcanic query wrapper for GetUserQuery
/// </summary>
public class GetUserArcanicQuery : IQuery<GetUserQueryResult>
{
    public int Id { get; init; }
}

/// <summary>
/// Arcanic query wrapper for SearchUsersQuery
/// </summary>
public class SearchUsersArcanicQuery : IQuery<SearchUsersQueryResult>
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

/// <summary>
/// Arcanic handler for GetUserQuery
/// </summary>
public class GetUserArcanicHandler : IQueryHandler<GetUserArcanicQuery, GetUserQueryResult>
{
    public Task<GetUserQueryResult> HandleAsync(GetUserArcanicQuery request, CancellationToken cancellationToken = default)
    {
        // Simulate data retrieval
        var result = new GetUserQueryResult
        {
            Id = request.Id,
            Name = $"User {request.Id}",
            Email = $"user{request.Id}@example.com"
        };

        return Task.FromResult(result);
    }
}

/// <summary>
/// Arcanic handler for SearchUsersQuery
/// </summary>
public class SearchUsersArcanicHandler : IQueryHandler<SearchUsersArcanicQuery, SearchUsersQueryResult>
{
    public Task<SearchUsersQueryResult> HandleAsync(SearchUsersArcanicQuery request, CancellationToken cancellationToken = default)
    {
        // Simulate search operation
        var users = Enumerable.Range(1, request.PageSize)
            .Select(i => new GetUserQueryResult
            {
                Id = i,
                Name = $"User {i}",
                Email = $"user{i}@example.com"
            });

        var result = new SearchUsersQueryResult
        {
            Users = users,
            TotalCount = 100 // Simulated total count
        };

        return Task.FromResult(result);
    }
}