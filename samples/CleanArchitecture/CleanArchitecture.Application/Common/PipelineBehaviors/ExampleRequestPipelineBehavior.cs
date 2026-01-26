using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Request.Abstractions.Pipeline;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.PipelineBehaviors;

/// <summary>
/// An example request pipeline behavior that demonstrates logging and performance monitoring capabilities in the request processing pipeline.
/// This behavior logs request execution information including the request name, correlation ID, and execution time.
/// </summary>
/// <typeparam name="TRequest">The type of request being processed. Must implement <see cref="IRequest"/>.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the request handler.</typeparam>
public class ExampleRequestPipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest
{
    /// <summary>
    /// The logger instance used to write request execution information.
    /// </summary>
    private readonly ILogger<ExampleRequestPipelineBehavior<TRequest, TResponse>> _logger;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ExampleRequestPipelineBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance used for writing log information.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
    public ExampleRequestPipelineBehavior(ILogger<ExampleRequestPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Handles the request execution by logging request information, measuring execution time, and then calling the next behavior in the pipeline.
    /// This method generates a correlation ID for tracking, logs the request execution start and completion, and measures performance.
    /// </summary>
    /// <param name="request">The request instance being processed.</param>
    /// <param name="next">The next delegate in the pipeline to invoke.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the response from the request handler.</returns>
    public async Task<TResponse> HandleAsync(TRequest request, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = Guid.NewGuid();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation("[REQUEST BEHAVIOR] Starting execution of {RequestName} with correlation ID {CorrelationId}", requestName, correlationId);
        
        try
        {
            var result = await next(cancellationToken);
            
            stopwatch.Stop();
            _logger.LogInformation("[REQUEST BEHAVIOR] Completed execution of {RequestName} with correlation ID {CorrelationId} in {ElapsedMilliseconds}ms", 
                requestName, correlationId, stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[REQUEST BEHAVIOR] Failed execution of {RequestName} with correlation ID {CorrelationId} after {ElapsedMilliseconds}ms", 
                requestName, correlationId, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
