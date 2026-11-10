<p align="center">
  <img src="https://raw.githubusercontent.com/arcanic-kit/mediator/main/assets/images/logo.png" alt="Arcanic Mediator logo" width="140" />
</p>

<h1 align="center">Arcanic Mediator</h1>

<p align="center">
  ✨ A fast, modular mediator for .NET — commands, queries and events with pipeline behaviors,<br />
  built on <code>Microsoft.Extensions.DependencyInjection</code>.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/Arcanic.Mediator"><img src="https://img.shields.io/nuget/v/Arcanic.Mediator?style=for-the-badge&logo=nuget&logoColor=white&label=NuGet&color=8258E0" alt="NuGet version" /></a>
  <a href="https://www.nuget.org/packages/Arcanic.Mediator"><img src="https://img.shields.io/nuget/dt/Arcanic.Mediator?style=for-the-badge&logo=nuget&logoColor=white&label=Downloads&color=9B45D6" alt="NuGet downloads" /></a>
  <a href="https://dotnet.microsoft.com"><img src="https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8 | 9 | 10" /></a>
  <a href="https://github.com/arcanic-kit/mediator/blob/main/LICENSE"><img src="https://img.shields.io/github/license/arcanic-kit/mediator?style=for-the-badge&color=B026B5" alt="MIT license" /></a>
  <a href="https://github.com/arcanic-kit/mediator"><img src="https://img.shields.io/github/stars/arcanic-kit/mediator?style=for-the-badge&logo=github&logoColor=white&label=Stars&color=8258E0" alt="GitHub stars" /></a>
</p>

## 🌟 Features

- 🧭 **CQRS out of the box** — commands, queries and events, each in its own package
- 🔗 **Pipeline behaviors** — for all messages, or just commands, queries or events
- 🪝 **Pre & post handlers** — validation, logging, caching around any handler
- 📣 **Fan-out events** — every handler runs, in registration order
- ⚡ **Fast** — dispatchers are cached per message type
- 🧪 **Easy to test** — mock `SendAsync` and `PublishAsync` directly

## 📦 Installation

Install only what you need:

```bash
dotnet add package Arcanic.Mediator.Command
dotnet add package Arcanic.Mediator.Query
dotnet add package Arcanic.Mediator.Event
```

## 🚀 Quick start

### 1️⃣ Register

```csharp
builder.Services.AddArcanicMediator()
    .AddCommands(Assembly.GetExecutingAssembly())
    .AddQueries(Assembly.GetExecutingAssembly())
    .AddEvents(Assembly.GetExecutingAssembly());
```

Handlers are discovered automatically in the assemblies you pass.

### 2️⃣ Write a command ✍️

```csharp
public record CreateProductCommand(string Name, decimal Price) : ICommand<int>;

public class CreateProductCommandHandler(IPublisher publisher) : ICommandHandler<CreateProductCommand, int>
{
    public async Task<int> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        var id = 42; // save the product...

        await publisher.PublishAsync(new ProductCreatedEvent(id, command.Name), cancellationToken);
        return id;
    }
}
```

> 💡 Use `ICommand` and `ICommandHandler<TCommand>` for commands that return nothing.

### 3️⃣ Write a query 🔍

```csharp
public record GetProductQuery(int Id) : IQuery<ProductDto>;

public class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductDto>
{
    public Task<ProductDto> HandleAsync(GetProductQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult(new ProductDto(query.Id, "Sample product", 19.99m));
}
```

### 4️⃣ Handle an event 📣

```csharp
public record ProductCreatedEvent(int Id, string Name) : IEvent;

public class SendWelcomeEmail : IEventHandler<ProductCreatedEvent>
{
    public Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"📧 New product: {@event.Name}");
        return Task.CompletedTask;
    }
}
```

An event can have any number of handlers — they all run, one after another, in registration order. Publishing an event with no handlers does nothing.

### 5️⃣ Send it 📬

```csharp
[ApiController]
[Route("products")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public Task<ProductDto> Get(int id) => mediator.SendAsync(new GetProductQuery(id));

    [HttpPost]
    public Task<int> Create(CreateProductCommand command) => mediator.SendAsync(command);
}
```

## 🪝 Pre & post handlers

Run extra logic before or after a specific handler — no registration needed beyond `AddCommands` / `AddQueries` / `AddEvents`:

```csharp
public class ValidateCreateProduct : ICommandPreHandler<CreateProductCommand>
{
    public Task HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Price <= 0) throw new ArgumentException("Price must be positive.");
        return Task.CompletedTask;
    }
}
```

Also available: `ICommandPostHandler<T>`, `IQueryPreHandler<T>`, `IQueryPostHandler<T>`, `IEventPreHandler<T>`, `IEventPostHandler<T>`.

## 🔗 Pipeline behaviors

Wrap messages with cross-cutting logic. Pick the scope you need:

| Interface | Runs for | Register with |
|-----------|----------|---------------|
| `IPipelineBehavior<,>` | 🌍 everything | `AddPipelineBehavior` |
| `IRequestPipelineBehavior<,>` | commands + queries | `AddRequestPipelineBehavior` |
| `ICommandPipelineBehavior<,>` | ✍️ commands | `AddCommandPipelineBehavior` |
| `IQueryPipelineBehavior<,>` | 🔍 queries | `AddQueryPipelineBehavior` |
| `IEventPipelineBehavior<,>` | 📣 events | `AddEventPipelineBehavior` |

```csharp
public class LoggingBehavior<TMessage, TResponse>(ILogger<LoggingBehavior<TMessage, TResponse>> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    public async Task<TResponse> HandleAsync(TMessage message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("▶️ {Message}", typeof(TMessage).Name);
        var response = await next(cancellationToken);
        logger.LogInformation("✅ {Message}", typeof(TMessage).Name);
        return response;
    }
}

builder.Services.AddArcanicMediator()
    .AddPipelineBehavior(typeof(LoggingBehavior<,>))
    .AddCommands(Assembly.GetExecutingAssembly());
```

**Execution order** — behaviors run in registration order, from the broadest scope to the narrowest:

```
🌍 Generic behaviors
  → Request behaviors
    → Command / Query / Event behaviors
      → 🪝 Pre-handlers
        → 🎯 Handler
      → 🪝 Post-handlers
```

## ⚙️ Configuration

Lifetimes default to `Transient`:

```csharp
builder.Services.AddArcanicMediator(config =>
{
    config.MediatorLifetime = InstanceLifetime.Scoped; // IMediator, IPublisher
    config.HandlerLifetime = InstanceLifetime.Scoped;  // handlers, pre/post handlers, behaviors
});
```

> ⚠️ Handlers that use scoped services (like a `DbContext`) should be `Scoped` or `Transient`. A `Singleton` mediator with `Scoped` handlers is rejected at startup.

## 🧪 Testing

`SendAsync` and `PublishAsync` are regular interface members of `IMediator` and `IPublisher`, so you can mock them directly with your favorite library.

### 🎭 Mock `IMediator` with Moq

```csharp
var mediator = new Mock<IMediator>();
mediator
    .Setup(m => m.SendAsync(It.IsAny<GetProductQuery>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(new ProductDto(1, "Test", 9.99m));

var controller = new ProductsController(mediator.Object);
var product = await controller.Get(1); // ✅ returns the mocked ProductDto

await controller.Create(new CreateProductCommand("Lamp", 10m));
mediator.Verify(m => m.SendAsync(
    It.Is<CreateProductCommand>(c => c.Name == "Lamp"),
    It.IsAny<CancellationToken>()), Times.Once);
```

### 🎭 Or with NSubstitute

```csharp
var mediator = Substitute.For<IMediator>();
mediator.SendAsync(Arg.Any<GetProductQuery>(), Arg.Any<CancellationToken>())
    .Returns(new ProductDto(1, "Test", 9.99m));

var controller = new ProductsController(mediator);

await controller.Create(new CreateProductCommand("Lamp", 10m));
await mediator.Received(1).SendAsync(
    Arg.Is<CreateProductCommand>(c => c.Name == "Lamp"),
    Arg.Any<CancellationToken>());
```

### 📣 Mock `IPublisher`

Check that a handler publishes the right event:

```csharp
var publisher = new Mock<IPublisher>();
var handler = new CreateProductCommandHandler(publisher.Object);

await handler.HandleAsync(new CreateProductCommand("Lamp", 10m));

publisher.Verify(p => p.PublishAsync(
    It.Is<ProductCreatedEvent>(e => e.Name == "Lamp"),
    It.IsAny<CancellationToken>()), Times.Once);
```

## 🧩 Packages

| Package | What it's for |
|---------|---------------|
| `Arcanic.Mediator.Command` | ✍️ Commands (writes) |
| `Arcanic.Mediator.Query` | 🔍 Queries (reads) |
| `Arcanic.Mediator.Event` | 📣 Events (notifications) |
| `Arcanic.Mediator` | ⚙️ Core registration — pulled in automatically |
| `*.Abstractions` | 🪶 Interfaces only — reference these from your domain/application layers |

## 📂 Sample

See [samples/CleanArchitecture](https://github.com/arcanic-kit/mediator/tree/main/samples/CleanArchitecture) for a complete Clean Architecture app:

```bash
cd samples/CleanArchitecture/CleanArchitecture.WebApi
dotnet run
```

## 💬 Support

- 🐛 [Report an issue](https://github.com/arcanic-kit/mediator/issues)
- 📝 [Release notes](https://github.com/arcanic-kit/mediator/releases)

## 📄 License

MIT — see [LICENSE](https://github.com/arcanic-kit/mediator/blob/main/LICENSE).
