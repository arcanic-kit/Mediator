using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Sample.WebApi.Application.Product.Queries.Get.PreHandlers;

/// <summary>
/// Pre-handler for validation GetProductQuery before the main handler executes.
/// This demonstrates cross-cutting concerns like input validation that should run before the main query logic.
/// </summary>
public class GetProductQueryValidationPreHandler : IQueryPreHandler<GetProductQuery>
{
    public async Task HandleAsync(GetProductQuery query, CancellationToken cancellationToken = default)
    {
        // Example validation logic
        await Task.Run(() => 
        {
            if (query.Id <= 0)
            {
                Console.WriteLine($"[PRE-HANDLER] Validation: Invalid Product ID: {query.Id}");
                throw new ArgumentException($"Product ID must be greater than 0. Received: {query.Id}", nameof(query.Id));
            }
            
            Console.WriteLine($"[PRE-HANDLER] Validation: GetProductQuery validation passed for ID: {query.Id}");
        }, cancellationToken);
    }
}