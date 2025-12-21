using Arcanic.Mediator.Abstractions.Pipeline;
using Microsoft.Extensions.Logging;

namespace Arcanic.Mediator.Sample.WebApi.Application.Common.Behaviors;

/// <summary>
/// A general-purpose logging behavior that can be applied to any message type.
/// This behavior logs the start and completion of message processing, including timing and exception handling.
/// </summary>
/// <typeparam name="TMessage">The type of message being processed.</typeparam>
/// <typeparam name="TResult">The type of result returned by the message processing.</typeparam>
public class LoggingPipelineBehavior<TMessage, TResult> : IRequestPipelineBehavior<TMessage, TResult>
    where TMessage : notnull
{
    private readonly ILogger<LoggingPipelineBehavior<TMessage, TResult>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingPipelineBehavior{TMessage,TResult}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for recording message processing details.</param>
    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TMessage, TResult>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the message processing with comprehensive logging including execution timing and exception handling.
    /// </summary>
    /// <param name="message">The message instance being processed.</param>
    /// <param name="next">The delegate representing the next behavior in the pipeline or the final handler execution.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The result of processing the message through this behavior and the remainder of the pipeline.</returns>
    public async Task<TResult> HandleAsync(TMessage message, PipelineDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        var messageName = typeof(TMessage).Name;
        var correlationId = Guid.NewGuid();

        _logger.LogInformation(
            "[BEHAVIOR] Starting execution of {MessageName} with correlation ID {CorrelationId}",
            messageName, correlationId);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await next();
            
            stopwatch.Stop();
            
            _logger.LogInformation(
                "[BEHAVIOR] Successfully completed {MessageName} with correlation ID {CorrelationId} in {ElapsedMilliseconds}ms",
                messageName, correlationId, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex,
                "[BEHAVIOR] Failed to execute {MessageName} with correlation ID {CorrelationId} after {ElapsedMilliseconds}ms",
                messageName, correlationId, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}