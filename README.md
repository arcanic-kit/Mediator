# Arcanic Mediator

A high-performance, modular mediator pattern implementation for .NET that provides clean separation of concerns through Command Query Responsibility Segregation (CQRS) and event-driven architecture.

## Features

- 🏗️ **Modular Architecture** - Register only the modules you need (Commands, Queries, Events)
- 🔧 **Clean CQRS Implementation** - Separate commands, queries, and events with dedicated mediators
- 🚀 **High Performance** - Minimal overhead with efficient message routing and cached dispatchers
- 📦 **Dependency Injection Ready** - First-class support for Microsoft.Extensions.DependencyInjection
- 🔍 **Auto-Discovery** - Automatically register handlers from assemblies
- ⚡ **Async/Await Support** - Full async support with CancellationToken propagation
- 🔀 **Pipeline Processing** - Pre/post handler support for cross-cutting concerns
- 🎨 **Clean Abstractions** - Separate abstraction packages for better dependency management

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
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add Arcanic Mediator with modules
builder.Services.AddArcanicMediator()
    .AddCommands(Assembly.GetExecutingAssembly())
    .AddQueries(Assembly.GetExecutingAssembly())
    .AddEvents(Assembly.GetExecutingAssembly());

var app = builder.Build();
```

### 2. Define Messages and Handlers

#### Commands

```csharp
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Event.Abstractions;

// Command without return value
public class CreateProductCommand : ICommand
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Command handler with return value
public class CreateProductCommandHandler : ICommandHandler<AddProductCommand, int>
{
    private readonly IPublisher _publisher;

    public CreateProductCommandHandler(IPublisher publisher)
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

> **Note**: Commands implement `ICommand` or `ICommand<TResult>`, which inherit from `IRequest` for unified mediator processing.

#### Queries

```csharp
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Handler;

public record ProductDto(int Id, string Name, decimal Price);

public class GetProductQuery : IQuery<ProductDto>
{
    public int Id { get; set; }
}

public class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductDto>
{
    public async Task<ProductDto> HandleAsync(GetProductQuery request, CancellationToken cancellationToken = default)
    {
        // Implementation here
        return await Task.FromResult(new ProductDto(request.Id, "Sample Product", 19.99m));
    }
}
```

> **Note**: Queries implement `IQuery<TResult>`, which inherits from `IRequest` for unified mediator processing.

#### Events

```csharp
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Handler;

public class ProductCreatedEvent : IEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

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

### 3. Use in Controllers

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

### Generic Pipeline exemple

The most universal pipeline that works with all message types - Commands, Queries, and Events:

```csharp
using Arcanic.Mediator.Abstractions.Pipeline;

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
