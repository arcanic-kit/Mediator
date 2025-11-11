using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Samples.WebApi.Application.Product.Queries.Get;

public class GetProductQueryHandler : IQueryHandler<GetProductQuery, GetProductQueryResponse>
{
    public async Task<GetProductQueryResponse> HandleAsync(GetProductQuery request, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(new GetProductQueryResponse
        {
            Id = request.Id,
            Name = "Sample Product",
            Price = 19.99m
        });
    }
}
