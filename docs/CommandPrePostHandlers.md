# Command Pre and Post Handlers

The Arcanic Mediator now supports pre-handlers and post-handlers for commands, enabling powerful cross-cutting concerns and pipeline processing.

## Overview

- **Pre-handlers** execute before the main command handler and are useful for validation, authentication, logging, or other preparatory operations
- **Post-handlers** execute after the main command handler completes and are useful for cleanup, caching, notifications, or other follow-up activities
- Multiple pre-handlers and post-handlers can be registered for the same command
- Pre-handlers execute in parallel before the main handler
- Post-handlers execute in parallel after the main handler and have access to the result

## Usage

### Defining Pre-handlers

```csharp
public class ValidateProductCommandPreHandler : ICommandPreHandler<AddProductCommand, AddProductCommandResponse>
{
    public async Task HandleAsync(AddProductCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Product name is required");
        
        // Additional validation logic
        await SomeAsyncValidation(command, cancellationToken);
    }
}
```

### Defining Post-handlers

```csharp
public class CacheProductCommandPostHandler : ICommandPostHandler<AddProductCommand, AddProductCommandResponse>
{
    public async Task HandleAsync(AddProductCommand command, AddProductCommandResponse result, CancellationToken cancellationToken = default)
    {
        // Cache the newly created product
        await _cache.SetAsync($"product_{result.Id}", result, cancellationToken);
        
        // Log the operation
        _logger.LogInformation("Product {ProductId} cached successfully", result.Id);
    }
}
```

### Command without Result

For commands that don't return a result:

```csharp
public class LogCommandPreHandler : ICommandPreHandler<DeleteProductCommand>
{
    public async Task HandleAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting product {ProductId}", command.ProductId);
        await Task.CompletedTask;
    }
}

public class CleanupCommandPostHandler : ICommandPostHandler<DeleteProductCommand>
{
    public async Task HandleAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        await _cacheService.InvalidateProduct(command.ProductId);
        _logger.LogInformation("Cleanup completed for product {ProductId}", command.ProductId);
    }
}
```

## Registration

Pre and post handlers are automatically discovered and registered when using assembly scanning:

```csharp
builder.Services.AddArcanicMediator(moduleRegistry =>
{
    moduleRegistry.AddCommandModule(commandModuleBuilder =>
    {
        commandModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
    });
});
```

## Execution Order

When a command is sent through the mediator:

1. **All pre-handlers execute in parallel** - If any pre-handler throws an exception, the pipeline stops
2. **Single main handler executes** - The primary business logic
3. **All post-handlers execute in parallel** - With access to the main handler's result (if any)

## Cross-Cutting Concerns

This pattern enables clean implementation of cross-cutting concerns:

### Authentication & Authorization
```csharp
public class AuthorizeProductCommandPreHandler : ICommandPreHandler<AddProductCommand, AddProductCommandResponse>
{
    private readonly ICurrentUser _currentUser;
    
    public async Task HandleAsync(AddProductCommand command, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.HasPermission("CreateProduct"))
            throw new UnauthorizedAccessException("User does not have permission to create products");
    }
}
```

### Validation
```csharp
public class ValidateProductCommandPreHandler : ICommandPreHandler<AddProductCommand, AddProductCommandResponse>
{
    private readonly IValidator<AddProductCommand> _validator;
    
    public async Task HandleAsync(AddProductCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(command, cancellationToken);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
    }
}
```

### Caching
```csharp
public class InvalidateCachePostHandler : ICommandPostHandler<UpdateProductCommand, ProductDto>
{
    private readonly IDistributedCache _cache;
    
    public async Task HandleAsync(UpdateProductCommand command, ProductDto result, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync($"product_{result.Id}", cancellationToken);
        await _cache.SetAsync($"product_{result.Id}", result, cancellationToken);
    }
}
```

### Event Publishing
```csharp
public class PublishProductEventsPostHandler : ICommandPostHandler<AddProductCommand, AddProductCommandResponse>
{
    private readonly IEventPublisher _eventPublisher;
    
    public async Task HandleAsync(AddProductCommand command, AddProductCommandResponse result, CancellationToken cancellationToken = default)
    {
        await _eventPublisher.PublishAsync(new ProductCreatedEvent 
        { 
            ProductId = result.Id, 
            ProductName = result.Name 
        }, cancellationToken);
    }
}
```

## Error Handling

- If any pre-handler throws an exception, the main handler and post-handlers will not execute
- If the main handler throws an exception, post-handlers will not execute
- If a post-handler throws an exception, other post-handlers will still execute, but the exception will propagate

## Best Practices

1. **Keep handlers focused** - Each handler should have a single responsibility
2. **Use pre-handlers for validation** - Fail fast before expensive operations
3. **Use post-handlers for side effects** - Logging, caching, event publishing
4. **Avoid dependencies between handlers** - Pre and post handlers should be independent
5. **Handle exceptions appropriately** - Consider whether failures should stop the pipeline
6. **Use async operations** - All handlers support cancellation tokens for better performance