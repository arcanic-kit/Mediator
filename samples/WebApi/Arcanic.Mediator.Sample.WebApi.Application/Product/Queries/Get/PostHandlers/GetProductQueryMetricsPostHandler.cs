using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Samples.WebApi.Application.Product.Queries.Get.PostHandlers;

/// <summary>
/// Post-handler for performance metrics after GetProductQuery completes successfully.
/// This demonstrates cross-cutting concerns like performance monitoring that should run after the main query logic.
/// </summary>
public class GetProductQueryMetricsPostHandler : IQueryPostHandler<GetProductQuery>
{
    public async Task HandleAsync(GetProductQuery query, CancellationToken cancellationToken = default)
    {
        // Example metrics logic
        await Task.Run(() => 
        {
            Console.WriteLine($"[POST-HANDLER] Metrics: GetProductQuery for ID {query.Id} completed - Recording performance metrics");
        }, cancellationToken);
    }
}