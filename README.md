# Arcanic Mediator

A powerful, modular mediator pattern implementation for .NET that provides clean separation of concerns through Command Query Responsibility Segregation (CQRS) and event-driven architecture.

## Features 

- 🏗️ **Modular Architecture** - Register only the modules you need (Commands, Queries, Events)
- 🔧 **Clean CQRS Implementation** - Separate commands, queries, and events with dedicated mediators
- 🚀 **High Performance** - Minimal overhead with efficient message routing
- 📦 **Dependency Injection Ready** - First-class support for Microsoft.Extensions.DependencyInjection
- 🔍 **Auto-Discovery** - Automatically register handlers from assemblies
- ⚡ **Async/Await Support** - Full async support with cancellation tokens
- 🎯 **Type Safe** - Strongly typed messages and handlers
- 📋 **Multiple Event Handlers** - Support for multiple handlers per event
- 🧩 **Extensible** - Easy to extend with custom strategies and behaviors
- 🔀 **Pipeline Processing** - Pre/post handler support for cross-cutting concerns
- 🎯 **Execution Strategies** - Configurable execution pipelines for different scenarios

## Installation

```bash
# Install the core package
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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddArcanicMediator(moduleRegistry =>
{   
    moduleRegistry.AddCommandModule(commandModuleBuilder =>
    {
        commandModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
    });

    moduleRegistry.AddQueryModule(queryModuleBuilder =>
    {
        queryModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
    });

    moduleRegistry.AddEventModule(eventModuleBuilder =>
    {
        eventModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
    });
});

var app = builder.Build();
```

### 2. Define Messages

#### Commands

```csharp
// Command without return value
public class CreateProductCommand : ICommand
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Command with return value
public class AddProductCommand : ICommand<AddProductCommandResponse>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class AddProductCommandResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

#### Queries

```csharp
public class GetProductQuery : IQuery<GetProductQueryResponse>
{
    public int Id { get; set; }
}

public class GetProductQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

#### Events

```csharp
public class ProductCreatedEvent : IEvent
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

### 3. Create Handlers

#### Command Handlers

```csharp
// Main command handler
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    public async Task HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        // Handle the command
        await SaveProductAsync(command.Name, command.Price);
    }
}

// Command handler with return value
public class AddProductCommandHandler : ICommandHandler<AddProductCommand, AddProductCommandResponse>
{
    public async Task<AddProductCommandResponse> HandleAsync(AddProductCommand command, CancellationToken cancellationToken = default)
    {
        var productId = await SaveProductAsync(command.Name, command.Price);
 
        return new AddProductCommandResponse
        {
            Id = productId,
            Name = command.Name
        };
    }
}

// Pre-handler for validation
public class AddProductCommandValidationPreHandler : ICommandPreHandler<AddProductCommand>
{
    public async Task HandleAsync(AddProductCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Product name cannot be empty");
        
        if (command.Price <= 0)
            throw new ArgumentException("Product price must be greater than zero");
            
        await Task.CompletedTask;
    }
}

// Post-handler for notifications
public class AddProductCommandNotificationPostHandler : ICommandPostHandler<AddProductCommand>
{
    public async Task HandleAsync(AddProductCommand command, CancellationToken cancellationToken = default)
    {
        // Send notifications after product creation
        await SendNotificationAsync($"Product '{command.Name}' has been created");
    }
}
```

#### Query Handlers

```csharp
// Main query handler
public class GetProductQueryHandler : IQueryHandler<GetProductQuery, GetProductQueryResponse>
{
    public async Task<GetProductQueryResponse> HandleAsync(GetProductQuery query, CancellationToken cancellationToken = default)
    {
        var product = await GetProductByIdAsync(query.Id);
        
        return new GetProductQueryResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
    }
}

// Pre-handler for validation
public class GetProductQueryValidationPreHandler : IQueryPreHandler<GetProductQuery>
{
    public async Task HandleAsync(GetProductQuery query, CancellationToken cancellationToken = default)
    {
        if (query.Id <= 0)
            throw new ArgumentException("Product ID must be greater than 0");
            
        await Task.CompletedTask;
    }
}

// Post-handler for caching
public class GetProductQueryCachingPostHandler : IQueryPostHandler<GetProductQuery>
{
    public async Task HandleAsync(GetProductQuery query, CancellationToken cancellationToken = default)
    {
        // Update cache after query execution
        await UpdateCacheAsync(query.Id);
    }
}
```

#### Event Handlers

```csharp
// Main event handlers (multiple allowed)
public class ProductCreatedEmailHandler : IEventHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Send notification email
        await SendEmailAsync(@event.ProductId, @event.ProductName);
    }
}

public class ProductCreatedLoggingHandler : IEventHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Log the event
        await LogEventAsync(@event);
    }
}

// Pre-handler for validation
public class ProductCreatedEventValidationPreHandler : IEventPreHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        if (@event.ProductId <= 0)
            throw new ArgumentException("Invalid Product ID");
            
        await Task.CompletedTask;
    }
}

// Post-handler for metrics
public class ProductCreatedEventMetricsPostHandler : IEventPostHandler<ProductCreatedEvent>
{
    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Record metrics after event processing
        await RecordMetricsAsync(@event);
    }
}
```

### 4. Use in Controllers

```csharp
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
    public async Task<AddProductCommandResponse> CreateProduct(AddProductCommand command)
    {
        var response = await _commandMediator.SendAsync(command);
    
        // Publish event
        await _eventPublisher.PublishAsync(new ProductCreatedEvent
        {
            ProductId = response.Id,
            ProductName = response.Name,
            CreatedAt = DateTime.UtcNow
        });

        return response;
    }
}
```

## Pipeline Execution

The Arcanic Mediator implements a sophisticated pipeline execution model that automatically coordinates pre-handlers, main handlers, and post-handlers:

### Execution Order

1. **Pre-handlers** - Execute before the main handler(s) for cross-cutting concerns like validation, authentication, logging
2. **Main handlers** - Execute the core business logic
3. **Post-handlers** - Execute after the main handler(s) for follow-up operations like caching, notifications, cleanup

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

- **Commands** - Represent actions that change state (write operations)
- **Queries** - Represent requests for data (read operations)  
- **Events** - Represent something that has happened (notifications)
- **Pipelines** - Coordinate pre/main/post handler execution
- **Strategies** - Define custom execution patterns

Each module can be used independently, allowing you to adopt only what you need.

## Advanced Features

### Cross-Cutting Concerns

Pre and post handlers are perfect for implementing cross-cutting concerns:

- **Validation** - Input validation in pre-handlers
- **Authentication/Authorization** - Security checks in pre-handlers
- **Logging** - Request/response logging in pre/post handlers
- **Caching** - Cache updates in post-handlers
- **Metrics** - Performance metrics collection in post-handlers
- **Notifications** - Event notifications in post-handlers

## Samples

Check out the [samples directory](./samples) for complete working examples including:

- Web API integration with pre/post handlers
- Advanced pipeline scenarios

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For questions and support:

- 📋 [Issues](https://github.com/arcanic-dotnet/mediator/issues)
- 📖 [Documentation](https://github.com/arcanic-dotnet/mediator/wiki)