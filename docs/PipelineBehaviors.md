# Pipeline Behaviors in Arcanic Mediator

Pipeline behaviors are a powerful feature that allows you to implement cross-cutting concerns in a composable and reusable way. Similar to MediatR's `IPipelineBehavior`, Arcanic Mediator's pipeline behaviors provide a way to wrap the execution of commands, queries, and events with additional logic.

## Overview

Pipeline behaviors in Arcanic Mediator work by creating a chain of responsibility around your message handlers. Each behavior can:
- Execute logic before the handler (pre-processing)
- Execute logic after the handler (post-processing)
- Modify the message or result
- Handle exceptions
- Short-circuit the execution pipeline

## Key Concepts

### IMessagePipelineBehavior<TMessage, TMessageResult>

This is the core interface for all pipeline behaviors:

```csharp
public interface IMessagePipelineBehavior<TMessage, TMessageResult> where TMessage : notnull
{
    Task<TMessageResult> HandleAsync(TMessage message, MessagePipelineDelegate<TMessageResult> next, CancellationToken cancellationToken = default);
}
```

### Specialized Interfaces

For convenience, there are specialized interfaces for different message types:

- `ICommandPipelineBehavior<TCommand>` - For commands without return values
- `ICommandPipelineBehavior<TCommand, TResult>` - For commands with return values  
- `IQueryPipelineBehavior<TQuery, TResult>` - For queries
- `IEventPipelineBehavior<TEvent>` - For events

## How It Works

Pipeline behaviors wrap around the existing pre-handler ? main handler ? post-handler execution flow:

```
Behavior 1 (start) ?
  Behavior 2 (start) ?
    Behavior 3 (start) ?
      Pre-handlers ?
        Main handler ?
      Post-handlers
    ? Behavior 3 (end)
  ? Behavior 2 (end)  
? Behavior 1 (end)
```

This allows behaviors to:
- Implement cross-cutting concerns that apply to the entire handler pipeline
- Wrap exceptions from any part of the execution
- Measure performance of the complete operation
- Implement caching around the entire operation

## Examples

### 1. Logging Behavior

```csharp
public class LoggingBehavior<TMessage, TResult> : IMessagePipelineBehavior<TMessage, TResult>
    where TMessage : notnull
{
    private readonly ILogger<LoggingBehavior<TMessage, TResult>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TMessage, TResult>> logger)
    {
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TMessage message, MessagePipelineDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        var messageName = typeof(TMessage).Name;
        var correlationId = Guid.NewGuid();

        _logger.LogInformation("Starting execution of {MessageName} with ID {CorrelationId}", messageName, correlationId);

        try
        {
            var result = await next();
            _logger.LogInformation("Completed execution of {MessageName} with ID {CorrelationId}", messageName, correlationId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed execution of {MessageName} with ID {CorrelationId}", messageName, correlationId);
            throw;
        }
    }
}
```

### 2. Validation Behavior

```csharp
public class ValidationBehavior<TCommand, TResult> : ICommandPipelineBehavior<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public async Task<TResult> HandleAsync(TCommand command, MessagePipelineDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        var validationContext = new ValidationContext(command);
        var validationResults = new List<ValidationResult>();
        
        if (!Validator.TryValidateObject(command, validationContext, validationResults, true))
        {
            var errors = validationResults.Select(vr => vr.ErrorMessage);
            throw new ValidationException($"Validation failed: {string.Join("; ", errors)}");
        }

        return await next();
    }
}
```

### 3. Caching Behavior (for Queries)

```csharp
public class CachingBehavior<TQuery, TResult> : IQueryPipelineBehavior<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    private readonly IMemoryCache _cache;

    public CachingBehavior(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<TResult> HandleAsync(TQuery query, MessagePipelineDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        var cacheKey = GenerateCacheKey(query);

        if (_cache.TryGetValue(cacheKey, out TResult? cachedResult) && cachedResult != null)
        {
            return cachedResult;
        }

        var result = await next();
        
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        
        return result;
    }

    private static string GenerateCacheKey(TQuery query) => $"{typeof(TQuery).Name}_{query.GetHashCode()}";
}
```

### 4. Performance Monitoring Behavior

```csharp
public class PerformanceMonitoringBehavior<TMessage, TResult> : IMessagePipelineBehavior<TMessage, TResult>
    where TMessage : notnull
{
    private readonly ILogger _logger;
    private readonly long _slowThresholdMs;

    public PerformanceMonitoringBehavior(ILogger<PerformanceMonitoringBehavior<TMessage, TResult>> logger, long slowThresholdMs = 500)
    {
        _logger = logger;
        _slowThresholdMs = slowThresholdMs;
    }

    public async Task<TResult> HandleAsync(TMessage message, MessagePipelineDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var result = await next();
            
            stopwatch.Stop();
            
            if (stopwatch.ElapsedMilliseconds > _slowThresholdMs)
            {
                _logger.LogWarning("Slow operation detected: {MessageName} took {ElapsedMs}ms", 
                    typeof(TMessage).Name, stopwatch.ElapsedMilliseconds);
            }
            
            return result;
        }
        catch
        {
            stopwatch.Stop();
            _logger.LogError("Operation failed after {ElapsedMs}ms: {MessageName}", 
                stopwatch.ElapsedMilliseconds, typeof(TMessage).Name);
            throw;
        }
    }
}
```

## Registration

Register behaviors in your dependency injection container:

```csharp
// Register for all messages
builder.Services.AddScoped(typeof(IMessagePipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddScoped(typeof(IMessagePipelineBehavior<,>), typeof(PerformanceMonitoringBehavior<,>));

// Register for specific message types
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped(typeof(IQueryPipelineBehavior<,>), typeof(CachingBehavior<,>));
```

## Execution Order

Behaviors are executed in the order they are registered in the dependency injection container. The last registered behavior will be the outermost wrapper:

```
Registration order: Logging ? Validation ? Performance
Execution order: Performance ? Validation ? Logging ? [Handlers] ? Logging ? Validation ? Performance
```

## Best Practices

### 1. Keep Behaviors Focused
Each behavior should have a single responsibility (logging, validation, caching, etc.).

### 2. Handle Exceptions Appropriately
```csharp
public async Task<TResult> HandleAsync(TMessage message, MessagePipelineDelegate<TResult> next, CancellationToken cancellationToken = default)
{
    try
    {
        // Pre-processing logic
        var result = await next();
        // Post-processing logic
        return result;
    }
    catch (SpecificException ex)
    {
        // Handle specific exceptions if needed
        throw;
    }
    // Let other exceptions bubble up
}
```

### 3. Consider Performance Impact
Behaviors add overhead to every message execution. Keep them lightweight and consider async operations carefully.

### 4. Use Cancellation Tokens
Always pass the cancellation token through the pipeline:

```csharp
public async Task<TResult> HandleAsync(TMessage message, MessagePipelineDelegate<TResult> next, CancellationToken cancellationToken = default)
{
    // Use cancellation token in your logic
    await SomeAsyncOperation(cancellationToken);
    
    return await next(); // The framework will pass the token to next()
}
```

### 5. Generic vs. Specific Behaviors
- Use generic behaviors (`IMessagePipelineBehavior<,>`) for cross-cutting concerns that apply to all messages
- Use specific behaviors (`ICommandPipelineBehavior<,>`) when you need type-specific logic

## Comparison with Pre/Post Handlers

| Aspect | Pipeline Behaviors | Pre/Post Handlers |
|--------|-------------------|------------------|
| **Scope** | Wraps entire execution (including other handlers) | Executes before/after main handler only |
| **Exception Handling** | Can catch exceptions from anywhere in pipeline | Limited to their own execution |
| **Composability** | Highly composable, chainable | Less composable |
| **Performance Monitoring** | Can measure complete operation | Can only measure partial operations |
| **Caching** | Can cache entire operation result | Limited caching capabilities |
| **Short-circuiting** | Can prevent handler execution | Cannot prevent main handler execution |

## When to Use Each

### Use Pipeline Behaviors for:
- Cross-cutting concerns that need to wrap the entire operation
- Exception handling and retry policies
- Performance monitoring and metrics
- Caching (especially for queries)
- Security checks that might prevent execution
- Request/response logging

### Use Pre/Post Handlers for:
- Domain-specific validation
- Business rule checks
- Data enrichment before processing
- Cleanup operations after processing
- Notifications and side effects

Both can coexist in the same application, providing maximum flexibility for implementing your cross-cutting concerns.