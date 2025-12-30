using Arcanic.Mediator.Query.Abstractions;

namespace CleanArchitecture.Application.Product.Queries.Get;

public class GetProductQuery: IQuery<GetProductQueryResponse>
{
    public int Id { get; set; }
}
