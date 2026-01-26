using System.Diagnostics;
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Pipeline;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.PipelineBehaviors;

/// <summary>
/// A performance monitoring behavior that measures and logs execution time for any message type.
/// This behavior can identify slow operations and provide insights into system performance.
/// </summary>
/// <typeparam name="TMessage">The type of message being processed.</typeparam>
/// <typeparam name="TResponse">The type of result returned by the message processing.</typeparam>
public class ExamplePipelineBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly ILogger<ExamplePipelineBehavior<TMessage, TResponse>> _logger;
    private readonly long _slowExecutionThresholdMs;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExamplePipelineBehavior{TMessage,TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for recording performance metrics.</param>
    /// <param name="slowExecutionThresholdMs">The threshold in milliseconds above which execution is considered slow. Default is 500ms.</param>
    public ExamplePipelineBehavior(
        ILogger<ExamplePipelineBehavior<TMessage, TResponse>> logger, 
        long slowExecutionThresholdMs = 500)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _slowExecutionThresholdMs = slowExecutionThresholdMs;
    }

    /// <summary>
    /// Handles the message processing with performance monitoring, measuring execution time
    /// and logging warnings for slow operations.
    /// </summary>
    /// <param name="message">The message instance being processed.</param>
    /// <param name="next">The delegate representing the next behavior in the pipeline or the final handler execution.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The result of processing the message through this behavior and the remainder of the pipeline.</returns>
    public async Task<TResponse> HandleAsync(TMessage message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var messageName = typeof(TMessage).Name;
        var operationId = Guid.NewGuid();
        var stopwatch = Stopwatch.StartNew();

        // Record memory before execution (optional - can be expensive)
        var memoryBefore = GC.GetTotalMemory(false);

        _logger.LogDebug(
            "[PERFORMANCE] Starting monitoring of {MessageName} with operation ID {OperationId}",
            messageName, operationId);

        try
        {
            var result = await next(cancellationToken);
            
            stopwatch.Stop();
            var memoryAfter = GC.GetTotalMemory(false);
            var memoryUsed = memoryAfter - memoryBefore;

            // Determine log level based on execution time
            var logLevel = stopwatch.ElapsedMilliseconds > _slowExecutionThresholdMs ? LogLevel.Warning : LogLevel.Information;
            
            _logger.Log(logLevel,
                "[PERFORMANCE] Completed {MessageName} with operation ID {OperationId} in {ElapsedMilliseconds}ms. Memory impact: {MemoryUsedBytes} bytes",
                messageName, operationId, stopwatch.ElapsedMilliseconds, memoryUsed);

            // Log additional warning for slow operations
            if (stopwatch.ElapsedMilliseconds > _slowExecutionThresholdMs)
            {
                _logger.LogWarning(
                    "[PERFORMANCE] SLOW OPERATION: {MessageName} took {ElapsedMilliseconds}ms, exceeding threshold of {ThresholdMs}ms",
                    messageName, stopwatch.ElapsedMilliseconds, _slowExecutionThresholdMs);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex,
                "[PERFORMANCE] Failed {MessageName} with operation ID {OperationId} after {ElapsedMilliseconds}ms",
                messageName, operationId, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}