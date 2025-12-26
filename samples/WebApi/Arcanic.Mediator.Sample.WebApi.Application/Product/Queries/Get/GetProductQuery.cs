using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Queries.Get;

public class GetProductQuery: IQuery<GetProductQueryResponse>
{
    public int Id { get; set; }
}
