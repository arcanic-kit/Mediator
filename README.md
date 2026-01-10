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

#### Queries

```csharp
using Arcanic.Mediator.Query.Abstractions;

public class GetProductQuery : IQuery<ProductDto>
{
    public int Id { get; set; }
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
    public async Task<ProductDto> GetProduct(int id)
    {
       return await _queryMediator.SendAsync(new GetProductQuery { Id = id });
    }

    [HttpPost]
    public async Task<int> CreateProduct(AddProductCommand command)
    {
        return await _commandMediator.SendAsync(command);
    }
    
    [HttpPost("simple")]
    public async Task CreateProductSimple(CreateProductCommand command)
    {
        await _commandMediator.SendAsync(command);
    }

    [HttpPost("event")]
    public async Task PublishEvent(ProductCreatedEvent @event)
    {
        await _eventPublisher.PublishAsync(@event);
    }
}
```

## Pipeline Behaviors

Add cross-cutting concerns with pipeline behaviors:

```csharp
// Add during configuration
builder.Services.AddArcanicMediator()
    .AddPipelineBehavior(typeof(LoggingPipelineBehavior<,>))
    .AddPipelineBehavior(typeof(ValidationPipelineBehavior<,>))
    .AddCommands(Assembly.GetExecutingAssembly())
    .AddQueries(Assembly.GetExecutingAssembly())
    .AddEvents(Assembly.GetExecutingAssembly());
```

## Architecture

The library follows a modular architecture with clear separation:

### Core Packages

- **Arcanic.Mediator** - Core dependency injection extensions and configuration
- **Arcanic.Mediator.Abstractions** - Common abstractions and pipeline interfaces

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
    .AddEvents(Assembly.GetExecutingAssembly();
```

### Key Differences

| Feature | MediatR | Arcanic Mediator |
|---------|---------|------------------|
| **Modularity** | Single package | Separate packages per feature |
| **Interface Names** | `IRequest<T>` | `ICommand<T>`, `IQuery<T>`, `IEvent` |
| **Mediator Interfaces** | `IMediator` | `ICommandMediator`, `IQueryMediator`, `IEventPublisher` |
| **Pre/Post Handlers** | Manual | Built-in support |
| **Performance** | Good | Optimized with cached dispatchers |

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
