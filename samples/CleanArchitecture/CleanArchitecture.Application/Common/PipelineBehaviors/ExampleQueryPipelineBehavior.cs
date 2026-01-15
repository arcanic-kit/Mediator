using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Pipeline;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.PipelineBehaviors;

/// <summary>
/// An example query pipeline behavior that demonstrates logging, caching consideration, and performance monitoring capabilities in the query processing pipeline.
/// This behavior logs query execution information including the query name, correlation ID, execution time, and result information.
/// It also demonstrates where caching logic could be implemented for read operations.
/// </summary>
/// <typeparam name="TQuery">The type of query being processed. Must implement <see cref="IQuery{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the query handler.</typeparam>
public class ExampleQueryPipelineBehavior<TQuery, TResponse> : IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// The logger instance used to write query execution information.
    /// </summary>
    private readonly ILogger<ExampleQueryPipelineBehavior<TQuery, TResponse>> _logger;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ExampleQueryPipelineBehavior{TQuery, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance used for writing log information.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
    public ExampleQueryPipelineBehavior(ILogger<ExampleQueryPipelineBehavior<TQuery, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Handles the query execution by logging query information, measuring execution time, and then calling the next behavior in the pipeline.
    /// This method generates a correlation ID for tracking, logs the query execution start and completion, measures performance,
    /// and provides hooks for caching logic implementation.
    /// </summary>
    /// <param name="query">The query instance being processed.</param>
    /// <param name="next">The next delegate in the pipeline to invoke.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the response from the query handler.</returns>
    /// <remarks>
    /// This behavior demonstrates common patterns for query processing including:
    /// - Request correlation tracking
    /// - Performance monitoring
    /// - Comprehensive logging for read operations
    /// - Error handling and logging
    /// - Placeholder for caching implementation
    /// </remarks>
    public async Task<TResponse> HandleAsync(TQuery query, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var queryName = typeof(TQuery).Name;
        var correlationId = Guid.NewGuid();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation("[QUERY BEHAVIOR] Starting execution of {QueryName} with correlation ID {CorrelationId}", 
            queryName, correlationId);
        
        // Note: This is where you could implement caching logic for queries
        // Example: Check cache first, return cached result if available
        
        try
        {
            _logger.LogDebug("[QUERY BEHAVIOR] Executing query handler for {QueryName} with correlation ID {CorrelationId}", 
                queryName, correlationId);
            
            var result = await next(cancellationToken);
            
            stopwatch.Stop();
            
            // Log additional information about the query result
            var resultInfo = result switch
            {
                null => "null result",
                System.Collections.IEnumerable enumerable => $"collection with {enumerable.Cast<object>().Count()} items",
                _ => $"result of type {result.GetType().Name}"
            };
            
            _logger.LogInformation("[QUERY BEHAVIOR] Completed execution of {QueryName} with correlation ID {CorrelationId} in {ElapsedMilliseconds}ms. Result: {ResultInfo}", 
                queryName, correlationId, stopwatch.ElapsedMilliseconds, resultInfo);
            
            // Note: This is where you could implement caching logic to store the result
            // Example: Store result in cache with appropriate expiration
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[QUERY BEHAVIOR] Failed execution of {QueryName} with correlation ID {CorrelationId} after {ElapsedMilliseconds}ms", 
                queryName, correlationId, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
