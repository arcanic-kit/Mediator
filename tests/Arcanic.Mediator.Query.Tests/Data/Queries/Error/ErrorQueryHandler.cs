using Arcanic.Mediator.Query.Abstractions.Handler;

namespace Arcanic.Mediator.Query.Tests.Data.Queries.Error;

public class ErrorQueryHandler : IQueryHandler<ErrorQuery, ErrorQueryResponse>
{
    public Task<ErrorQueryResponse> HandleAsync(ErrorQuery query, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException(query.ErrorMessage);
    }
}