using Arcanic.Mediator.Benchmarks.Models;
using MediatR;

namespace Arcanic.Mediator.Benchmarks.MediatR.Queries;

/// <summary>
/// MediatR wrapper for GetUserQuery
/// </summary>
public record GetUserRequest(int Id) : IRequest<GetUserQueryResult>;

/// <summary>
/// MediatR wrapper for SearchUsersQuery
/// </summary>
public record SearchUsersRequest(string? Name, string? Email, int Page = 1, int PageSize = 10) 
    : IRequest<SearchUsersQueryResult>;

/// <summary>
/// MediatR handler for GetUserQuery
/// </summary>
public class GetUserHandler : IRequestHandler<GetUserRequest, GetUserQueryResult>
{
    public Task<GetUserQueryResult> Handle(GetUserRequest request, CancellationToken cancellationToken)
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
/// MediatR handler for SearchUsersQuery
/// </summary>
public class SearchUsersHandler : IRequestHandler<SearchUsersRequest, SearchUsersQueryResult>
{
    public Task<SearchUsersQueryResult> Handle(SearchUsersRequest request, CancellationToken cancellationToken)
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