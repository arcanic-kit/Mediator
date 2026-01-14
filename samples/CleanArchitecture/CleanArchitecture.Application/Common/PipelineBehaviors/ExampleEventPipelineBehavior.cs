using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Pipeline;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.PipelineBehaviors;

/// <summary>
/// An example event pipeline behavior that demonstrates logging, audit trailing, and error handling capabilities in the event processing pipeline.
/// This behavior logs event execution information including the event name, correlation ID, execution time, and processing results.
/// It also demonstrates where audit logging and event sourcing logic could be implemented for domain events.
/// </summary>
/// <typeparam name="TEvent">The type of event being processed. Must implement <see cref="IEvent"/>.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the event handlers (typically <see cref="Unit"/> for fire-and-forget operations).</typeparam>
public class ExampleEventPipelineBehavior<TEvent, TResponse> : IEventPipelineBehavior<TEvent, TResponse>
    where TEvent : IEvent
{
    /// <summary>
    /// The logger instance used to write event execution information.
    /// </summary>
    private readonly ILogger<ExampleEventPipelineBehavior<TEvent, TResponse>> _logger;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ExampleEventPipelineBehavior{TEvent, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance used for writing log information.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
    public ExampleEventPipelineBehavior(ILogger<ExampleEventPipelineBehavior<TEvent, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Handles the event execution by logging event information, measuring execution time, and then calling the next behavior in the pipeline.
    /// This method generates a correlation ID for tracking, logs the event processing start and completion, measures performance,
    /// and provides hooks for audit logging and event sourcing implementation.
    /// </summary>
    /// <param name="event">The event instance being processed.</param>
    /// <param name="next">The next delegate in the pipeline to invoke.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the response from the event handlers.</returns>
    /// <remarks>
    /// This behavior demonstrates common patterns for event processing including:
    /// - Event correlation tracking for distributed systems
    /// - Performance monitoring for event handlers
    /// - Comprehensive audit logging for domain events
    /// - Error handling and recovery logging
    /// - Placeholder for event sourcing and audit trail implementation
    /// - Support for fire-and-forget event processing patterns
    /// </remarks>
    public async Task<TResponse> HandleAsync(TEvent @event, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var eventName = typeof(TEvent).Name;
        var correlationId = Guid.NewGuid();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var timestamp = DateTimeOffset.UtcNow;
        
        _logger.LogInformation("[EVENT BEHAVIOR] Processing event {EventName} with correlation ID {CorrelationId} at {Timestamp}", 
            eventName, correlationId, timestamp);
        
        // Note: This is where you could implement audit logging for events
        // Example: Store event in audit log or event store for compliance/replay purposes
        
        // Note: This is where you could implement event validation
        // Example: Validate event payload against business rules or schema
        
        try
        {
            _logger.LogDebug("[EVENT BEHAVIOR] Executing event handlers for {EventName} with correlation ID {CorrelationId}", 
                eventName, correlationId);
            
            var result = await next(cancellationToken);
            
            stopwatch.Stop();
            
            // Log success information about the event processing
            var processingInfo = result switch
            {
                null => "no response (fire-and-forget)",
                var r when r.Equals(default(TResponse)) => "default response",
                _ => $"response of type {result.GetType().Name}"
            };
            
            _logger.LogInformation("[EVENT BEHAVIOR] Successfully processed event {EventName} with correlation ID {CorrelationId} in {ElapsedMilliseconds}ms. Processing result: {ProcessingInfo}", 
                eventName, correlationId, stopwatch.ElapsedMilliseconds, processingInfo);
            
            // Note: This is where you could implement post-processing audit logging
            // Example: Log successful event processing completion for audit trails
            
            // Note: This is where you could trigger follow-up events or notifications
            // Example: Publish integration events to external systems
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Enhanced error logging for events with context information
            _logger.LogError(ex, "[EVENT BEHAVIOR] Failed to process event {EventName} with correlation ID {CorrelationId} after {ElapsedMilliseconds}ms. Error: {ErrorMessage}", 
                eventName, correlationId, stopwatch.ElapsedMilliseconds, ex.Message);
            
            // Note: This is where you could implement error event publishing
            // Example: Publish error events for monitoring and alerting systems
            
            // Note: This is where you could implement retry logic or dead letter queuing
            // Example: Send failed events to retry queue or dead letter queue
            
            throw;
        }
    }
}
