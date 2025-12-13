using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Queries.Get;

public class GetProductQueryHandler: IQueryHandler<GetProductQuery, GetProductQueryResponse>
{
    public async Task<GetProductQueryResponse> HandleAsync(GetProductQuery request, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(
            new GetProductQueryResponse()
            {
                Product = new(request.Id, "Sample Product", 19.99m)
            }
        );
    }
}
