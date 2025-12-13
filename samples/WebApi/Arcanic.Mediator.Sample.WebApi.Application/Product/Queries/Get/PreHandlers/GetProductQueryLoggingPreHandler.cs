using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Queries.Get.PreHandlers;

/// <summary>
/// Pre-handler for logging GetProductQuery before the main handler executes.
/// This demonstrates cross-cutting concerns like auditing/logging that should run before the main query logic.
/// </summary>
public class GetProductQueryLoggingPreHandler : IQueryPreHandler<GetProductQuery>
{
    public async Task HandleAsync(GetProductQuery query, CancellationToken cancellationToken = default)
    {
        // Example logging logic
        await Task.Run(() => 
        {
            Console.WriteLine($"[PRE-HANDLER] Logging: GetProductQuery received - ID: {query.Id}");
        }, cancellationToken);
    }
}