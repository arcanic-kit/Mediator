# Arcanic Mediator

A high-performance, modular mediator pattern implementation for .NET that provides clean separation of concerns through Command Query Responsibility Segregation (CQRS) and event-driven architecture.

## Features 

- 🏗️ **Modular Architecture** - Register only the modules you need (Commands, Queries, Events)
- 🔧 **Clean CQRS Implementation** - Separate commands, queries, and events with dedicated mediators
- 🚀 **High Performance** - Minimal overhead with efficient message routing and dispatching
- 📦 **Dependency Injection Ready** - First-class support for Microsoft.Extensions.DependencyInjection
- 🔍 **Auto-Discovery** - Automatically register handlers from assemblies using reflection
- ⚡ **Async/Await Support** - Full async support with CancellationToken propagation
- 🎯 **Type Safe** - Strongly typed messages and handlers with compile-time safety
- 📋 **Multiple Event Handlers** - Support for multiple handlers per event with parallel execution
- 🧩 **Extensible** - Easy to extend with custom pipeline behaviors and strategies
- 🔀 **Pipeline Processing** - Pre/post handler support for cross-cutting concerns
- 🎯 **Pipeline Behaviors** - Composable pipeline behaviors for logging, validation, caching, etc.
- 🎨 **Clean Abstractions** - Separate abstraction packages for better dependency management
- 🌐 **Multi-targeting** - Supports .NET 8, .NET 9, and .NET 10

## Installation

Install the packages you need based on your requirements:

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
builder.Services.AddArcanicMediator(moduleRegistry =>
{
    // Register command module with auto-discovery
    moduleRegistry.AddCommandModule(commandBuilder => 
    {
        commandBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
    });

    // Register query module with auto-discovery
    moduleRegistry.AddQueryModule(queryBuilder => 
    {
        queryBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
    });

    // Register event module with auto-discovery
    moduleRegistry.AddEventModule(eventBuilder => 
    {
        eventBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
    });
});

// Optional: Add pipeline behaviors for cross-cutting concerns
builder.Services.AddTransient(typeof(IRequestPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

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

#### Queries

```csharp
using Arcanic.Mediator.Query.Abstractions;

public class GetProductQuery : IQuery<GetProductQueryResponse>
{
    public int Id { get; set; }
}

public class GetProductQueryResponse
{
    public ProductDto Product { get; set; } = null!;
}

public record ProductDto(int Id, string Name, decimal Price);
```

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
    private readonly IEventPublisher _eventPublisher;

    public AddProductCommandHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task<int> HandleAsync(AddProductCommand request, CancellationToken cancellationToken = default)
    {
        // Save product and get ID
        var productId = await SaveProductAsync(request.Name, request.Price);
        
        // Publish domain event
        await _eventPublisher.PublishAsync(new ProductCreatedEvent
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
        return await Task.FromResult(1);
    }
}

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
        await SendNotificationAsync($"Product '{request.Name}' has been created");
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

// Main query handler
public class GetProductQueryHandler : IQueryHandler<GetProductQuery, GetProductQueryResponse>
{
    public async Task<GetProductQueryResponse> HandleAsync(GetProductQuery request, CancellationToken cancellationToken = default)
    {
        var product = await GetProductByIdAsync(request.Id);
        
        return new GetProductQueryResponse
        {
            Product = new ProductDto(product.Id, product.Name, product.Price)
        };
    }
    
    private async Task<ProductDto> GetProductByIdAsync(int id)
    {
        // Implementation here
        return await Task.FromResult(new ProductDto(id, "Sample Product", 19.99m));
    }
}

// Pre-handler for validation
public class GetProductQueryValidationPreHandler : IQueryPreHandler<GetProductQuery>
{
    public async Task HandleAsync(GetProductQuery request, CancellationToken cancellationToken = default)
    {
        if (request.Id <= 0)
            throw new ArgumentException("Product ID must be greater than 0");
            
        await Task.CompletedTask;
    }
}

// Post-handler for caching
public class GetProductQueryCachingPostHandler : IQueryPostHandler<GetProductQuery>
{
    public async Task HandleAsync(GetProductQuery request, CancellationToken cancellationToken = default)
    {
        // Update cache after query execution
        await UpdateCacheAsync(request.Id);
    }
    
    private async Task UpdateCacheAsync(int productId)
    {
        // Implementation here
        await Task.CompletedTask;
    }
}
```

#### Event Handlers

```csharp
using Arcanic.Mediator.Event.Abstractions.Handler;

// Main event handlers (multiple allowed)
public class ProductCreatedEmailHandler : IEventHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent request, CancellationToken cancellationToken = default)
    {
        // Send notification email
        await SendEmailAsync(request.Id, request.Name);
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
        await LogEventAsync(request);
    }
    
    private async Task LogEventAsync(ProductCreatedEvent @event)
    {
        // Implementation here
        await Task.CompletedTask;
    }
}

// Pre-handler for validation
public class ProductCreatedEventValidationPreHandler : IEventPreHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent request, CancellationToken cancellationToken = default)
    {
        if (request.Id == Guid.Empty)
            throw new ArgumentException("Invalid Product ID");
            
        await Task.CompletedTask;
    }
}

// Post-handler for metrics
public class ProductCreatedEventMetricsPostHandler : IEventPostHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent request, CancellationToken cancellationToken = default)
    {
        // Record metrics after event processing
        await RecordMetricsAsync(request);
    }
    
    private async Task RecordMetricsAsync(ProductCreatedEvent @event)
    {
        // Implementation here
        await Task.CompletedTask;
    }
}
```

### 4. Use in Controllers

```csharp
using Microsoft.AspNetCore.Mvc;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Event.Abstractions;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ICommandMediator _commandMediator;
    private readonly IQueryMediator _queryMediator;
    private readonly IEventPublisher _eventPublisher;

    public ProductController(
        ICommandMediator commandMediator, 
        IQueryMediator queryMediator,
        IEventPublisher eventPublisher)
    {
        _commandMediator = commandMediator;
        _queryMediator = queryMediator;
        _eventPublisher = eventPublisher;
    }

    [HttpGet("{id}")]
    public async Task<GetProductQueryResponse> GetProduct(int id)
    {
       return await _queryMediator.SendAsync(new GetProductQuery { Id = id });
    }

    [HttpPost]
    public async Task<int> CreateProduct(AddProductCommand command)
    {
        var productId = await _commandMediator.SendAsync(command);
        return productId;
    }
    
    [HttpPost("simple")]
    public async Task CreateProductSimple(CreateProductCommand command)
    {
        await _commandMediator.SendAsync(command);
    }
}
```

## Pipeline Behaviors

Arcanic Mediator supports pipeline behaviors that allow you to implement cross-cutting concerns in a composable way. Pipeline behaviors wrap around the execution of commands, queries, and events.

### Creating a Pipeline Behavior

```csharp
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Pipeline;
using Microsoft.Extensions.Logging;

public class LoggingPipelineBehavior<TRequest, TResult> : IRequestPipelineBehavior<TRequest, TResult>
    where TRequest : IRequest
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResult>> _logger;

    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResult>> logger)
    {
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TRequest request, PipelineDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        var messageName = typeof(TRequest).Name;
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
                "[BEHAVIOR] Failed execution of {MessageName} with correlation ID {CorrelationId} after {ElapsedMilliseconds}ms",
                messageName, correlationId, stopwatch.ElapsedMilliseconds);
                
            throw;
        }
    }
}
```

### Registering Pipeline Behaviors

```csharp
// Register pipeline behaviors in DI container
builder.Services.AddTransient(typeof(IRequestPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
builder.Services.AddTransient(typeof(IRequestPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
builder.Services.AddTransient(typeof(IRequestPipelineBehavior<,>), typeof(PerformanceMonitoringPipelineBehavior<,>));
```

## Pipeline Execution

The Arcanic Mediator implements a sophisticated pipeline execution model that automatically coordinates pre-handlers, main handlers, and post-handlers:

### Execution Order

1. **Pipeline Behaviors** - Execute in registration order (outermost first)
2. **Pre-handlers** - Execute before the main handler(s) for cross-cutting concerns like validation, authentication, logging
3. **Main handlers** - Execute the core business logic
4. **Post-handlers** - Execute after the main handler(s) for follow-up operations like caching, notifications, cleanup

### Handler Types

#### Commands
- **Main Handler**: Single handler that processes the command
- **Pre-handlers**: Multiple handlers for validation, authentication, etc.
- **Post-handlers**: Multiple handlers for notifications, cleanup, etc.

#### Queries  
- **Main Handler**: Single handler that returns the query result
- **Pre-handlers**: Multiple handlers for validation, authentication, etc.
- **Post-handlers**: Multiple handlers for caching, metrics, etc.

#### Events
- **Main Handlers**: Multiple handlers that process the event independently
- **Pre-handlers**: Multiple handlers for validation, filtering, etc.
- **Post-handlers**: Multiple handlers for cleanup, metrics, etc.

## Architecture

The library follows a modular architecture with clear separation of concerns:

### Core Packages

- **Arcanic.Mediator** - Core dependency injection extensions and module registry
- **Arcanic.Mediator.Abstractions** - Common abstractions and pipeline interfaces

### Message Packages

- **Arcanic.Mediator.Command** / **Arcanic.Mediator.Command.Abstractions** - Command handling (write operations)
- **Arcanic.Mediator.Query** / **Arcanic.Mediator.Query.Abstractions** - Query handling (read operations)  
- **Arcanic.Mediator.Event** / **Arcanic.Mediator.Event.Abstractions** - Event publishing (notifications)

### Key Concepts

- **Commands** - Represent actions that change state (write operations)
- **Queries** - Represent requests for data (read operations)  
- **Events** - Represent something that has happened (notifications)
- **Pipeline Behaviors** - Cross-cutting concerns that wrap message execution
- **Pre/Post Handlers** - Handlers that execute before/after main handlers

Each module can be used independently, allowing you to adopt only what you need for your specific use case.

## Performance

Arcanic Mediator is designed for high performance with minimal overhead:

- **Efficient Message Routing** - Direct handler resolution without reflection lookups
- **Optimized Pipelines** - Streamlined execution paths for pre/main/post handlers
- **Minimal Allocations** - Reduced garbage collection pressure
- **Parallel Event Processing** - Multiple event handlers execute concurrently
- **Configurable Behaviors** - Only pay for the features you use

### Benchmarks

The library includes comprehensive benchmarks comparing performance across different scenarios:

```bash
# Run performance benchmarks
cd benchmarks/Arcanic.Mediator.Benchmarks
dotnet run -c Release

# Available benchmark categories:
# - query: Query processing performance
# - command: Command processing performance  
# - event: Event publishing performance
# - all: Run all benchmarks
```

Typical performance characteristics:
- **Commands**: ~100-500ns per operation (simple handlers)
- **Queries**: ~100-500ns per operation (simple handlers)
- **Events**: ~1-5μs per event (with multiple handlers)

## Advanced Features

### Cross-Cutting Concerns

Pre and post handlers are perfect for implementing cross-cutting concerns:

- **Validation** - Input validation in pre-handlers
- **Authentication/Authorization** - Security checks in pre-handlers  
- **Logging** - Request/response logging in pre/post handlers
- **Caching** - Cache invalidation and updates in post-handlers
- **Metrics** - Performance and business metrics collection
- **Auditing** - Change tracking and audit logging
- **Rate Limiting** - Request throttling and quotas
- **Error Handling** - Centralized exception handling and recovery
- **Notifications** - Event notifications in post-handlers

### Custom Pipeline Behaviors

Create reusable behaviors for complex cross-cutting concerns:

```csharp
public class ValidationPipelineBehavior<TMessage, TResult> : IRequestPipelineBehavior<TMessage, TResult>
    where TMessage : notnull
{
    private readonly IValidator<TMessage> _validator;

    public ValidationPipelineBehavior(IValidator<TMessage> validator)
    {
        _validator = validator;
    }

    public async Task<TResult> HandleAsync(TMessage message, PipelineDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        // Validate before processing
        var validationResult = await _validator.ValidateAsync(message, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Continue with pipeline
        return await next();
    }
}
```

## Best Practices

### Message Design

```csharp
// ✅ Good: Immutable record types
public record CreateProductCommand(string Name, decimal Price) : ICommand<int>;

// ✅ Good: Clear naming conventions
public record GetProductByIdQuery(int ProductId) : IQuery<ProductDto>;

// ✅ Good: Specific event names
public record ProductCreatedEvent(Guid ProductId, string ProductName, decimal Price) : IEvent;

// ❌ Avoid: Generic names
public record DataCommand(object Data) : ICommand; // Too generic
```

### Handler Organization

```csharp
// ✅ Good: One handler per file, clear naming
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, int>
{
    // Implementation
}

// ✅ Good: Grouped by feature/aggregate
// Features/Products/Commands/Create/CreateProductCommand.cs
// Features/Products/Commands/Create/CreateProductCommandHandler.cs
// Features/Products/Queries/GetById/GetProductByIdQuery.cs
// Features/Products/Queries/GetById/GetProductByIdQueryHandler.cs
```

### Dependency Injection

```csharp
// ✅ Good: Use abstraction packages in business layer
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Query.Abstractions;

// ✅ Good: Use implementation packages only in composition root
// Program.cs or Startup.cs:
using Arcanic.Mediator.Command;
using Arcanic.Mediator.Query;
```

### Error Handling

```csharp
// ✅ Good: Specific exception types
public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(int productId) 
        : base($"Product with ID {productId} was not found") { }
}

// ✅ Good: Use pipeline behaviors for common error handling
public class ExceptionHandlingPipelineBehavior<TRequest, TResult> : IRequestPipelineBehavior<TRequest, TResult>
    where TRequest : IRequest
{
    public async Task<TResult> HandleAsync(TRequest request, PipelineDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        try
        {
            return await next();
        }
        catch (DomainException ex)
        {
            // Handle domain-specific exceptions
            throw new BusinessLogicException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            // Log and rethrow
            // _logger.LogError(ex, "Unexpected error processing {RequestType}", typeof(TRequest).Name);
            throw;
        }
    }
}
```

### Testing

```csharp
// ✅ Good: Test handlers directly
[Test]
public async Task CreateProductCommandHandler_Should_Create_Product()
{
    // Arrange
    var handler = new CreateProductCommandHandler(_mockRepository.Object);
    var command = new CreateProductCommand("Test Product", 19.99m);

    // Act
    var result = await handler.HandleAsync(command, CancellationToken.None);

    // Assert
    Assert.That(result, Is.GreaterThan(0));
}

// ✅ Good: Integration tests for full pipeline
[Test]
public async Task CreateProduct_Should_Process_Full_Pipeline()
{
    // Arrange - set up test services with real mediator

    // Act
    var result = await _commandMediator.SendAsync(command);

    // Assert - verify end-to-end behavior
}
```

## Samples

Check out the [samples directory](./samples) for complete working examples:

### Clean Architecture Sample

The [Clean Architecture sample](./samples/CleanArchitecture) demonstrates a Clean Architecture implementation with:

- **Domain Layer** - Core business entities and events
- **Application Layer** - Commands, queries, and handlers  
- **Infrastructure Layer** - Data access and external services
- **WebApi Layer** - Controllers and API endpoints

Key features demonstrated:
- Command and query handlers
- Event publishing and handling
- Pre/post handlers for validation and logging
- Pipeline behaviors for cross-cutting concerns
- Dependency injection setup

### Running the Sample

```bash
cd samples/WebApi/CleanArchitecture.WebApi
dotnet run
```

Navigate to `https://localhost:5001/swagger` to explore the API endpoints.

### Sample Endpoints

```bash
# Get a product
GET /Product/1

# Create a product
POST /Product
{
    "name": "Sample Product",
    "price": 29.99
}

# Update product price
PUT /Product/1/price
{
    "newPrice": 39.99
}
```

## Migration from Other Mediator Libraries

### From MediatR

Arcanic Mediator provides a similar API with enhanced modularity:

```csharp
// MediatR
services.AddMediatR(Assembly.GetExecutingAssembly());

// Arcanic Mediator - More granular control
services.AddArcanicMediator(moduleRegistry =>
{
    moduleRegistry
        .AddCommandModule(cmd => cmd.RegisterFromAssembly(Assembly.GetExecutingAssembly()))
        .AddQueryModule(qry => qry.RegisterFromAssembly(Assembly.GetExecutingAssembly()))
        .AddEventModule(evt => evt.RegisterFromAssembly(Assembly.GetExecutingAssembly()));
});
```

#### Key Differences

| Feature | MediatR | Arcanic Mediator                    |
|---------|---------|-------------------------------------|
| **Modularity** | Single package | Separate packages for Commands/Queries/Events |
| **Interface Names** | `IRequest<T>` | `ICommand<T>`, `IQuery<T>`, `IEvent` |
| **Mediator Interfaces** | `IMediator` | `ICommandMediator`, `IQueryMediator`, `IEventPublisher` |
| **Pipeline Behaviors** | `IPipelineBehavior<T,R>` | `IRequestPipelineBehavior<T,R>`, `IPipelineBehavior<T,R>` |
| **Pre/Post Handlers** | Manual implementation | Built-in support                    |
| **Performance** | Good | Optimized for minimal overhead      |

#### Migration Example

```csharp
// MediatR style
public class GetProductQuery : IRequest<ProductDto>
{
    public int Id { get; set; }
}

public class GetProductHandler : IRequestHandler<GetProductQuery, ProductDto>
{
    public Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}

// Arcanic Mediator style
public class GetProductQuery : IQuery<ProductDto> 
{
    public int Id { get; set; }
}

public class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductDto>
{
    public Task<ProductDto> HandleAsync(GetProductQuery request, CancellationToken cancellationToken = default)
    {
        // Implementation
    }
}
```

## Contributing

We welcome contributions! Here's how you can help:

### Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/arcanic-kit/mediator.git
   cd mediator
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run tests**
   ```bash
   dotnet test
   ```

5. **Run benchmarks**
   ```bash
   cd benchmarks/Arcanic.Mediator.Benchmarks
   dotnet run -c Release
   ```

### Contribution Guidelines

- **Code Style**: Follow existing code conventions and naming patterns
- **Testing**: Add comprehensive tests for new features
- **Documentation**: Update documentation for API changes
- **Performance**: Consider performance implications and add benchmarks for new features
- **Backwards Compatibility**: Avoid breaking changes in minor versions

### Areas for Contribution

- **New Pipeline Behaviors** - Common cross-cutting concerns
- **Performance Optimizations** - Micro-optimizations and memory usage improvements  
- **Documentation** - Examples, tutorials, and API documentation
- **Testing** - Unit tests, integration tests, and performance tests
- **Samples** - Real-world usage examples and scenarios

### Reporting Issues

Please use the [GitHub Issues](https://github.com/arcanic-kit/mediator/issues) page to report bugs or request features. Include:

- Clear description of the issue or feature request
- Steps to reproduce (for bugs)
- Expected vs actual behavior
- Relevant code samples
- Environment details (.NET version, OS, etc.)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For questions and support:

- 📋 [Issues](https://github.com/arcanic-kit/mediator/issues)
- 📖 [Documentation](https://github.com/arcanic-kit/mediator)
