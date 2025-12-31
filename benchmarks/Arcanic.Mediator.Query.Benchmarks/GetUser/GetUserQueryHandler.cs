using System.Threading;
using System.Threading.Tasks;
using Arcanic.Mediator.Query.Abstractions.Handler;

namespace Arcanic.Mediator.Query.Benchmarks.GetUser;

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
