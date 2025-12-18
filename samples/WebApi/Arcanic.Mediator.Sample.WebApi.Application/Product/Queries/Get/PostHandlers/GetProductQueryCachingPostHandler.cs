using Arcanic.Mediator.Query.Abstractions.Handler;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Queries.Get.PostHandlers;

/// <summary>
/// Post-handler for caching operations after GetProductQuery completes successfully.
/// This demonstrates cross-cutting concerns like caching that should run after the main query logic.
/// </summary>
public class GetProductQueryCachingPostHandler : IQueryPostHandler<GetProductQuery>
{
    public async Task HandleAsync(GetProductQuery query, CancellationToken cancellationToken = default)
    {
        // Example caching logic
        await Task.Run(() => 
        {
            Console.WriteLine($"[POST-HANDLER] Caching: Product query result for ID {query.Id} cached successfully");
        }, cancellationToken);
    }
}