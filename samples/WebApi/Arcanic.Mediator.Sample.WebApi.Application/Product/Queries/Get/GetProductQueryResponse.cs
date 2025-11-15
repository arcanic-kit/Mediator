namespace Arcanic.Mediator.Samples.WebApi.Application.Product.Queries.Get;

public class GetProductQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
