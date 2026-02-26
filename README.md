# Arcanic Mediator

A high-performance, modular mediator pattern implementation for .NET that provides clean separation of concerns through Command Query Responsibility Segregation (CQRS) and event-driven architecture.

## Features

- 🏗️ **Modular Architecture** - Register only the modules you need (Commands, Queries, Events)
- 🔧 **Clean CQRS Implementation** - Separate commands, queries, and events with dedicated mediators
- 🚀 **High Performance** - Minimal overhead with efficient message routing and cached dispatchers
- 📦 **Dependency Injection Ready** - First-class support for Microsoft.Extensions.DependencyInjection
- 🔍 **Auto-Discovery** - Automatically register handlers from assemblies
- ⚡ **Async/Await Support** - Full async support with CancellationToken propagation
- 🎯 **Type Safe** - Strongly typed messages and handlers with compile-time safety
- 📋 **Multiple Event Handlers** - Support for multiple handlers per event with parallel execution
- 🔀 **Pipeline Processing** - Pre/post handler support for cross-cutting concerns
- 🎨 **Clean Abstractions** - Separate abstraction packages for better dependency management
- 🌐 **Multi-targeting** - Supports .NET 8, .NET 9, and .NET 10

## Installation

Install the packages you need:

```bash
dotnet add package Arcanic.Mediator.Command
dotnet add package Arcanic.Mediator.Query
dotnet add package Arcanic.Mediator.Event
```

## Quick Start

### 1. Configure Services

```csharp
using Arcanic.Mediator;
using Arcanic.Mediator.Command;
using Arcanic.Mediator.Query;
using Arcanic.Mediator.Event;
using Arcanic.Mediator.Request;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add Arcanic Mediator with modules
builder.Services.AddArcanicMediator()
    .AddCommands(Assembly.GetExecutingAssembly())
    .AddQueries(Assembly.GetExecutingAssembly())
    .AddEvents(Assembly.GetExecutingAssembly());

var app = builder.Build();
```

### 2. Define Messages

#### Commands

```csharp
using Arcanic.Mediator.Command.Abstractions;

// Command without return value
public class CreateProductCommand : ICommand
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Command with return value
public class AddProductCommand : ICommand<int>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

> **Note**: Commands implement `ICommand` or `ICommand<TResult>`, which inherit from `IRequest` for unified mediator processing.

#### Queries

```csharp
using Arcanic.Mediator.Query.Abstractions;

public class GetProductQuery : IQuery<ProductDto>
{
    public int Id { get; set; }
}

public record ProductDto(int Id, string Name, decimal Price);
```

> **Note**: Queries implement `IQuery<TResult>`, which inherits from `IRequest` for unified mediator processing.

#### Events

```csharp
using Arcanic.Mediator.Event.Abstractions;

public class ProductCreatedEvent : IEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

### 3. Create Handlers

#### Command Handlers

```csharp
using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Event.Abstractions;

// Main command handler
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    public async Task HandleAsync(CreateProductCommand request, CancellationToken cancellationToken = default)
    {
        // Handle the command
        await SaveProductAsync(request.Name, request.Price);
    }
    
    private async Task SaveProductAsync(string name, decimal price)
    {
        // Implementation here
        await Task.CompletedTask;
    }
}

// Command handler with return value
public class AddProductCommandHandler : ICommandHandler<AddProductCommand, int>
{
    private readonly IPublisher _publisher;

    public AddProductCommandHandler(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task<int> HandleAsync(AddProductCommand request, CancellationToken cancellationToken = default)
    {
        // Save product and get ID
        var productId = await SaveProductAsync(request.Name, request.Price);

        // Publish domain event
        await _publisher.PublishAsync(new ProductCreatedEvent
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price
        }, cancellationToken);

        return productId;
    }

    private async Task<int> SaveProductAsync(string name, decimal price)
    {
        // Implementation here
        await Task.Delay(100); // Simulate async work
        return 1;
    }
}
```

#### Pre/Post Handlers

```csharp
// Pre-handler for validation
public class AddProductCommandValidationPreHandler : ICommandPreHandler<AddProductCommand>
{
    public async Task HandleAsync(AddProductCommand request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Product name cannot be empty");
        
        if (request.Price <= 0)
            throw new ArgumentException("Product price must be greater than zero");
            
        await Task.CompletedTask;
    }
}

// Post-handler for notifications
public class AddProductCommandNotificationPostHandler : ICommandPostHandler<AddProductCommand>
{
    public async Task HandleAsync(AddProductCommand request, CancellationToken cancellationToken = default)
    {
        // Send notifications after product creation
        Console.WriteLine($"Product '{request.Name}' has been created");
        await Task.CompletedTask;
    }
    
    private async Task SendNotificationAsync(string message)
    {
        // Implementation here
        await Task.CompletedTask;
    }
}
```

#### Query Handlers

```csharp
using Arcanic.Mediator.Query.Abstractions.Handler;

public class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductDto>
{
    public async Task<ProductDto> HandleAsync(GetProductQuery request, CancellationToken cancellationToken = default)
    {
        // Implementation here
        return await Task.FromResult(new ProductDto(request.Id, "Sample Product", 19.99m));
    }
}
```

#### Event Handlers

```csharp
using Arcanic.Mediator.Event.Abstractions.Handler;

// Multiple event handlers can exist for the same event
public class ProductCreatedEmailHandler : IEventHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent request, CancellationToken cancellationToken = default)
    {
        // Send notification email
        Console.WriteLine($"Sending email for product: {request.Name}");
        await Task.CompletedTask;
    }
    
    private async Task SendEmailAsync(Guid productId, string productName)
    {
        // Implementation here
        await Task.CompletedTask;
    }
}

public class ProductCreatedLoggingHandler : IEventHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent request, CancellationToken cancellationToken = default)
    {
        // Log the event
        Console.WriteLine($"Product created: {request.Id} - {request.Name}");
        await Task.CompletedTask;
    }
}
```

### 4. Use in Controllers

```csharp
using Microsoft.AspNetCore.Mvc;
using Arcanic.Mediator.Request.Abstractions;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ProductDto> GetProduct(int id)
    {
       return await _mediator.SendAsync(new GetProductQuery { Id = id });
    }

    [HttpPost]
    public async Task<int> CreateProduct(AddProductCommand command)
    {
        return await _mediator.SendAsync(command);
    }

    [HttpPost("simple")]
    public async Task CreateProductSimple(CreateProductCommand command)
    {
        await _mediator.SendAsync(command);
    }
}
```

## Pipeline Behaviors

Arcanic Mediator provides a powerful pipeline system that allows you to implement cross-cutting concerns through different types of pipeline behaviors. Each message type (Commands, Queries, Events, and generic Requests) has its own dedicated pipeline interface, enabling type-safe and context-aware processing.

### Pipeline Types and Their Differences

| Pipeline Type | Interface | Purpose | Key Characteristics |
|---------------|-----------|---------|-------------------|
| **Generic Pipeline** | `IPipelineBehavior<TMessage, TResponse>` | Universal message processing | Works with all message types (Commands, Queries, Events), most flexible |
| **Request Pipeline** | `IRequestPipelineBehavior<TRequest, TResponse>` | Commands and queries processing | Works with `ICommand` and `IQuery` messages, shared concerns |
| **Command Pipeline** | `ICommandPipelineBehavior<TCommand, TResponse>` | Write operations processing | Optimized for state changes, transaction support |
| **Query Pipeline** | `IQueryPipelineBehavior<TQuery, TResponse>` | Read operations processing | Caching-aware, performance monitoring focused |
| **Event Pipeline** | `IEventPipelineBehavior<TEvent, TResponse>` | Domain event processing | Audit logging, event sourcing support |

### When to Use Each Pipeline Type

- **Generic Pipeline**: Use for cross-cutting concerns that apply to ALL message types including events (correlation tracking, global error handling, metrics collection)
- **Request Pipeline**: Use for cross-cutting concerns that apply to both commands and queries only (request validation, authorization)
- **Command Pipeline**: Use for write-specific concerns (transactions, authorization for modifications, business rule validation)
- **Query Pipeline**: Use for read-specific optimizations (caching, query performance monitoring, read authorization)
- **Event Pipeline**: Use for event-specific concerns (audit trails, event sourcing, notification reliability)

### Generic Pipeline (Universal)

The most universal pipeline that works with all message types - Commands, Queries, and Events:

```csharp
using Arcanic.Mediator.Abstractions.Pipeline;

public class GlobalMetricsPipelineBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull
{
    private readonly IMetricsCollector _metricsCollector;
    private readonly ILogger<GlobalMetricsPipelineBehavior<TMessage, TResponse>> _logger;

    public GlobalMetricsPipelineBehavior(
        IMetricsCollector metricsCollector,
        ILogger<GlobalMetricsPipelineBehavior<TMessage, TResponse>> logger)
    {
        _metricsCollector = metricsCollector;
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TMessage message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var messageName = typeof(TMessage).Name;
        var correlationId = Guid.NewGuid();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Start metrics collection
        using var activity = _metricsCollector.StartActivity(messageName);
        
        _logger.LogDebug("[GLOBAL] Processing {MessageName} with correlation ID {CorrelationId}", 
            messageName, correlationId);

        try
        {
            var result = await next(cancellationToken);
            
            stopwatch.Stop();
            
            // Record successful execution metrics
            _metricsCollector.RecordExecution(messageName, stopwatch.Elapsed, success: true);
            
            _logger.LogDebug("[GLOBAL] Completed {MessageName} in {ElapsedMs}ms with correlation ID {CorrelationId}", 
                messageName, stopwatch.ElapsedMilliseconds, correlationId);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Record failed execution metrics
            _metricsCollector.RecordExecution(messageName, stopwatch.Elapsed, success: false);
            
            _logger.LogError(ex, "[GLOBAL] Failed {MessageName} after {ElapsedMs}ms with correlation ID {CorrelationId}", 
                messageName, stopwatch.ElapsedMilliseconds, correlationId);
            
            throw;
        }
    }
}

// Global error handling pipeline
public class GlobalExceptionPipelineBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull
{
    private readonly ILogger<GlobalExceptionPipelineBehavior<TMessage, TResponse>> _logger;

    public GlobalExceptionPipelineBehavior(ILogger<GlobalExceptionPipelineBehavior<TMessage, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TMessage message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (Exception ex)
        {
            var messageName = typeof(TMessage).Name;
            
            // Log with structured data for monitoring systems
            _logger.LogError(ex, 
                "[GLOBAL ERROR] Unhandled exception in {MessageName}. Message: {@Message}",
                messageName, message);

            // You could implement circuit breaker, dead letter queue, etc. here
            
            throw; // Re-throw to maintain the original exception flow
        }
    }
}
```

### Request Pipeline (Generic)

The most flexible pipeline that handles both commands and queries:

```csharp
using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Request.Abstractions.Pipeline;

public class LoggingRequestPipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest
{
    private readonly ILogger<LoggingRequestPipelineBehavior<TRequest, TResponse>> _logger;

    public LoggingRequestPipelineBehavior(ILogger<LoggingRequestPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TRequest request, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = Guid.NewGuid();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        _logger.LogInformation("[REQUEST] Starting {RequestName} with correlation ID {CorrelationId}", 
            requestName, correlationId);

        try
        {
            var result = await next(cancellationToken);
            
            stopwatch.Stop();
            _logger.LogInformation("[REQUEST] Completed {RequestName} in {ElapsedMs}ms with correlation ID {CorrelationId}", 
                requestName, stopwatch.ElapsedMilliseconds, correlationId);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[REQUEST] Failed {RequestName} after {ElapsedMs}ms with correlation ID {CorrelationId}", 
                requestName, stopwatch.ElapsedMilliseconds, correlationId);
            throw;
        }
    }
}
```

### Command Pipeline

Specialized pipeline for command processing with transaction support:

```csharp
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Pipeline;

public class TransactionCommandPipelineBehavior<TCommand, TResponse> : ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand
{
    private readonly IDbContextTransaction _transaction;
    private readonly ILogger<TransactionCommandPipelineBehavior<TCommand, TResponse>> _logger;

    public TransactionCommandPipelineBehavior(
        IDbContextTransaction transaction,
        ILogger<TransactionCommandPipelineBehavior<TCommand, TResponse>> logger)
    {
        _transaction = transaction;
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TCommand command, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        
        _logger.LogInformation("[COMMAND] Starting transaction for {CommandName}", commandName);

        try
        {
            var result = await next(cancellationToken);
            
            await _transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("[COMMAND] Transaction committed for {CommandName}", commandName);
            
            return result;
        }
        catch (Exception ex)
        {
            await _transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "[COMMAND] Transaction rolled back for {CommandName}", commandName);
            throw;
        }
    }
}

// Authorization pipeline for commands
public class AuthorizationCommandPipelineBehavior<TCommand, TResponse> : ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand
{
    private readonly ICurrentUser _currentUser;
    private readonly IAuthorizationService _authorizationService;

    public AuthorizationCommandPipelineBehavior(ICurrentUser currentUser, IAuthorizationService authorizationService)
    {
        _currentUser = currentUser;
        _authorizationService = authorizationService;
    }

    public async Task<TResponse> HandleAsync(TCommand command, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(
            _currentUser.User, command, typeof(TCommand).Name);

        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedAccessException($"User not authorized to execute {typeof(TCommand).Name}");
        }

        return await next(cancellationToken);
    }
}
```

### Query Pipeline

Optimized pipeline for query processing with caching support:

```csharp
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Pipeline;

public class CachingQueryPipelineBehavior<TQuery, TResponse> : IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingQueryPipelineBehavior<TQuery, TResponse>> _logger;

    public CachingQueryPipelineBehavior(IMemoryCache cache, ILogger<CachingQueryPipelineBehavior<TQuery, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TQuery query, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{typeof(TQuery).Name}_{query.GetHashCode()}";
        
        // Try to get from cache first
        if (_cache.TryGetValue(cacheKey, out TResponse cachedResult))
        {
            _logger.LogInformation("[QUERY] Cache hit for {QueryName}", typeof(TQuery).Name);
            return cachedResult;
        }

        _logger.LogInformation("[QUERY] Cache miss for {QueryName}, executing query", typeof(TQuery).Name);

        // Execute query and cache result
        var result = await next(cancellationToken);
        
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };

        _cache.Set(cacheKey, result, cacheOptions);
        
        return result;
    }
}

// Performance monitoring pipeline for queries
public class PerformanceQueryPipelineBehavior<TQuery, TResponse> : IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly ILogger<PerformanceQueryPipelineBehavior<TQuery, TResponse>> _logger;

    public PerformanceQueryPipelineBehavior(ILogger<PerformanceQueryPipelineBehavior<TQuery, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TQuery query, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var queryName = typeof(TQuery).Name;

        try
        {
            var result = await next(cancellationToken);
            
            stopwatch.Stop();
            
            if (stopwatch.ElapsedMilliseconds > 1000) // Log slow queries
            {
                _logger.LogWarning("[QUERY] Slow query detected: {QueryName} took {ElapsedMs}ms", 
                    queryName, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation("[QUERY] {QueryName} completed in {ElapsedMs}ms", 
                    queryName, stopwatch.ElapsedMilliseconds);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[QUERY] {QueryName} failed after {ElapsedMs}ms", 
                queryName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
```

### Event Pipeline

Specialized pipeline for event processing with audit and reliability features:

```csharp
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Pipeline;

public class AuditEventPipelineBehavior<TEvent, TResponse> : IEventPipelineBehavior<TEvent, TResponse>
    where TEvent : IEvent
{
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditEventPipelineBehavior<TEvent, TResponse>> _logger;

    public AuditEventPipelineBehavior(IAuditService auditService, ILogger<AuditEventPipelineBehavior<TEvent, TResponse>> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TEvent @event, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var eventName = typeof(TEvent).Name;
        var correlationId = Guid.NewGuid();

        // Create audit entry before processing
        await _auditService.LogEventStartAsync(eventName, @event, correlationId, cancellationToken);
        
        _logger.LogInformation("[EVENT] Processing {EventName} with correlation ID {CorrelationId}", 
            eventName, correlationId);

        try
        {
            var result = await next(cancellationToken);
            
            // Log successful processing
            await _auditService.LogEventCompletionAsync(eventName, correlationId, success: true, cancellationToken);
            
            return result;
        }
        catch (Exception ex)
        {
            // Log failed processing
            await _auditService.LogEventCompletionAsync(eventName, correlationId, success: false, cancellationToken);
            _logger.LogError(ex, "[EVENT] Failed to process {EventName} with correlation ID {CorrelationId}", 
                eventName, correlationId);
            throw;
        }
    }
}

// Reliability pipeline for events (retry logic)
public class ReliabilityEventPipelineBehavior<TEvent, TResponse> : IEventPipelineBehavior<TEvent, TResponse>
    where TEvent : IEvent
{
    private readonly ILogger<ReliabilityEventPipelineBehavior<TEvent, TResponse>> _logger;

    public ReliabilityEventPipelineBehavior(ILogger<ReliabilityEventPipelineBehavior<TEvent, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TEvent @event, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        const int maxRetries = 3;
        var eventName = typeof(TEvent).Name;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await next(cancellationToken);
            }
            catch (Exception ex) when (attempt < maxRetries && IsRetriableException(ex))
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)); // Exponential backoff
                
                _logger.LogWarning(ex, "[EVENT] Attempt {Attempt}/{MaxRetries} failed for {EventName}, retrying in {DelaySeconds}s", 
                    attempt, maxRetries, eventName, delay.TotalSeconds);
                
                await Task.Delay(delay, cancellationToken);
            }
        }

        // Final attempt without catching exception
        return await next(cancellationToken);
    }

    private static bool IsRetriableException(Exception ex)
    {
        // Define which exceptions are retriable (network issues, temporary failures, etc.)
        return ex is HttpRequestException || 
               ex is TaskCanceledException || 
               ex is SocketException;
    }
}
```

### Pipeline Registration and Configuration

Register different pipeline behaviors based on your needs:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Arcanic Mediator with all pipeline types
builder.Services.AddArcanicMediator()
    // Generic pipelines (apply to ALL message types - Commands, Queries, Events)
    .AddPipelineBehavior(typeof(GlobalMetricsPipelineBehavior<,>))
    .AddPipelineBehavior(typeof(GlobalExceptionPipelineBehavior<,>))

    // Request pipelines (apply to commands and queries)
    .AddRequestPipelineBehavior(typeof(LoggingRequestPipelineBehavior<,>))

    // Command-specific pipelines
    .AddCommandPipelineBehavior(typeof(AuthorizationCommandPipelineBehavior<,>))
    .AddCommandPipelineBehavior(typeof(TransactionCommandPipelineBehavior<,>))

    // Query-specific pipelines
    .AddQueryPipelineBehavior(typeof(CachingQueryPipelineBehavior<,>))
    .AddQueryPipelineBehavior(typeof(PerformanceQueryPipelineBehavior<,>))

    // Event-specific pipelines
    .AddEventPipelineBehavior(typeof(AuditEventPipelineBehavior<,>))
    .AddEventPipelineBehavior(typeof(ReliabilityEventPipelineBehavior<,>))

    // Register modules
    .AddCommands(Assembly.GetExecutingAssembly())
    .AddQueries(Assembly.GetExecutingAssembly())
    .AddEvents(Assembly.GetExecutingAssembly());

var app = builder.Build();
```

### Pipeline Execution Order

Pipelines execute in the order they are registered:

1. **Generic Pipelines** (outermost) - Execute for all message types (Commands, Queries, Events)
2. **Request Pipelines** - Execute for commands and queries
3. **Type-specific Pipelines** (Command/Query/Event) - Execute based on message type
4. **Pre-handlers** - Execute before main handler
5. **Main handler** - Executes the core business logic
6. **Post-handlers** - Execute after main handler

Example execution flow for a command:
```
Generic Pipeline (Metrics) 
  → Generic Pipeline (Exception Handling)
    → Request Pipeline (Logging) 
      → Command Pipeline (Authorization)
        → Command Pipeline (Transaction)
          → Pre-handler (Validation)
            → Main Handler (Business Logic)
          → Post-handler (Notifications)
```

Example execution flow for an event:
```
Generic Pipeline (Metrics) 
  → Generic Pipeline (Exception Handling)
    → Event Pipeline (Audit)
      → Event Pipeline (Reliability/Retry)
        → Event Handlers (Multiple, in parallel)
```

## Configuration

Configure mediator services with custom settings:

```csharp
// Configure service lifetime (default is Transient)
builder.Services.AddArcanicMediator(config =>
{
    config.InstanceLifetime = InstanceLifetime.Scoped; // or Singleton, Transient
})
.AddCommands(Assembly.GetExecutingAssembly())
.AddQueries(Assembly.GetExecutingAssembly())
.AddEvents(Assembly.GetExecutingAssembly());
```

### ArcanicMediatorConfiguration Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `InstanceLifetime` | `InstanceLifetime` | `Transient` | Controls how mediator service instances are created and managed by the DI container |

**Instance Lifetime Options:**
- **Transient** - New instance created each time (default)
- **Scoped** - One instance per request/scope
- **Singleton** - Single instance for the application lifetime

## Architecture

The library follows a modular architecture with clear separation:

### Core Packages

- **Arcanic.Mediator** - Core dependency injection extensions and configuration
- **Arcanic.Mediator.Abstractions** - Common abstractions and pipeline interfaces
- **Arcanic.Mediator.Request** / **Request.Abstractions** - Unified request processing and mediation

### Feature Packages

- **Arcanic.Mediator.Command** / **Command.Abstractions** - Command handling (write operations)
- **Arcanic.Mediator.Query** / **Query.Abstractions** - Query handling (read operations)  
- **Arcanic.Mediator.Event** / **Event.Abstractions** - Event publishing (notifications)

### Key Concepts

- **Commands** - Actions that change state (write operations)
- **Queries** - Requests for data (read operations)  
- **Events** - Things that have happened (notifications)
- **Pre/Post Handlers** - Cross-cutting concerns that execute before/after main handlers
- **Pipeline Behaviors** - Reusable behaviors that wrap message execution

## Pipeline Execution Order

1. **Pipeline Behaviors** - Execute in registration order (outermost first)
2. **Pre-handlers** - Execute before main handler for validation, authentication, etc.
3. **Main handler** - Executes the core business logic
4. **Post-handlers** - Execute after main handler for notifications, cleanup, etc.

## Performance

- **Cached Dispatchers** - Avoid reflection overhead with cached dispatcher instances
- **Efficient Routing** - Direct handler resolution without runtime type discovery
- **Minimal Allocations** - Optimized for low garbage collection pressure
- **Parallel Events** - Multiple event handlers execute concurrently

Run benchmarks:
```bash
cd benchmarks/Arcanic.Mediator.Command.Benchmarks
dotnet run -c Release
```

## Samples

Check out the [samples/CleanArchitecture](./samples/CleanArchitecture) for a complete working example demonstrating:
- Clean Architecture implementation
- Command and query handlers
- Event publishing and handling
- Pre/post handlers
- Pipeline behaviors

Run the sample:
```bash
cd samples/CleanArchitecture/CleanArchitecture.WebApi
dotnet run
```

## Migration from MediatR

Arcanic Mediator provides a similar API with enhanced modularity:

```csharp
// MediatR
services.AddMediatR(Assembly.GetExecutingAssembly());

// Arcanic Mediator
services.AddArcanicMediator()
    .AddCommands(Assembly.GetExecutingAssembly())
    .AddQueries(Assembly.GetExecutingAssembly())
    .AddEvents(Assembly.GetExecutingAssembly());
```

### Key Differences

| Feature | MediatR | Arcanic Mediator |
|---------|---------|------------------|
| **Modularity** | Single package | Separate packages per feature |
| **Interface Names** | `IRequest<T>` | `ICommand<T>`, `IQuery<T>`, `IEvent` |
| **Mediator Interfaces** | `IMediator` | `IMediator` |
| **Event Publishing** | `IMediator.Publish()` | Dedicated `IPublisher` interface |
| **Pre/Post Handlers** | Manual | Built-in support |
| **Performance** | Good | Optimized with cached dispatchers |
| **Architecture** | Monolithic | Modular with clear separation of concerns |

## Contributing

1. Clone the repository
2. Run `dotnet restore`
3. Run `dotnet build`
4. Run `dotnet test`

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- 📋 [Issues](https://github.com/arcanic-kit/mediator/issues)
- 📖 [Documentation](https://github.com/arcanic-kit/mediator)
