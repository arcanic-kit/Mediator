using Arcanic.Mediator.Query.Abstractions.Handler;
using CleanArchitecture.Domain.Products;

namespace CleanArchitecture.Application.Product.Queries.Get;

public class GetProductQueryHandler(IProductRepository productRepository) : IQueryHandler<GetProductQuery, GetProductQueryResponse>
{
    public async Task<GetProductQueryResponse> HandleAsync(GetProductQuery request, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(request.Id);

        return new GetProductQueryResponse
        {
            Product = product
        };
    }
}
