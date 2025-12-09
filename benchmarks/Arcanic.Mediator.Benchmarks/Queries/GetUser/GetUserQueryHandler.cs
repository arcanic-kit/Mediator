using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Benchmarks.Queries.GetUser;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, GetUserQueryResponse>
{
    public Task<GetUserQueryResponse> HandleAsync(GetUserQuery request, CancellationToken cancellationToken = default)
    {
        // Simulate data retrieval
        var result = new GetUserQueryResponse
        {
            Id = request.Id,
            Name = $"User {request.Id}",
            Email = $"user{request.Id}@example.com"
        };

        return Task.FromResult(result);
    }
}
