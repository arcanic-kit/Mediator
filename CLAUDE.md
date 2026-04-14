# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build Arcanic.Mediator.slnx

# Run all tests
dotnet test Arcanic.Mediator.slnx

# Run tests in a specific project
dotnet test tests/Arcanic.Mediator.Command.Tests/Arcanic.Mediator.Command.Tests.csproj

# Run a single test by name
dotnet test tests/Arcanic.Mediator.Command.Tests/Arcanic.Mediator.Command.Tests.csproj --filter "FullyQualifiedName~<TestMethodName>"

# Pack NuGet packages (Release)
dotnet pack Arcanic.Mediator.slnx --configuration Release

# Run benchmarks
cd benchmarks/Arcanic.Mediator.Command.Benchmarks && dotnet run -c Release
```

## Architecture

Arcanic.Mediator is a high-performance, modular mediator pattern library for .NET targeting CQRS and event-driven architectures. The codebase is split into composable NuGet packages under `src/`.

### Package Structure

| Package | Role |
|---|---|
| `Arcanic.Mediator.Abstractions` | Core abstractions: `IMessage`, `IPipelineBehavior<TMessage, TResponse>` |
| `Arcanic.Mediator` | DI wiring: `AddArcanicMediator()`, `DefaultArcanicMediatorBuilder` |
| `Arcanic.Mediator.Command.Abstractions` | `ICommand`, `ICommand<T>`, `ICommandHandler` interfaces |
| `Arcanic.Mediator.Command` | `CommandDispatcher` (cached), DI registration |
| `Arcanic.Mediator.Query.Abstractions` | `IQuery<T>`, `IQueryHandler<TQuery, TResponse>` interfaces |
| `Arcanic.Mediator.Query` | `QueryDispatcher` (cached), DI registration |
| `Arcanic.Mediator.Event.Abstractions` | `IEvent`, `IEventHandler`, `IPublisher` interfaces |
| `Arcanic.Mediator.Event` | `EventDispatcher` (parallel concurrent execution), DI registration |
| `Arcanic.Mediator.Request.Abstractions` | Unified `IMediator` entry point, `IRequest` abstraction |
| `Arcanic.Mediator.Request` | Unified request implementation delegating to command/query dispatchers |

### Execution Flow

```
IMediator.SendAsync / PublishAsync
  └─ CommandDispatcher / QueryDispatcher / EventPublisher
       └─ Pipeline chain (ordered):
            Generic (IPipelineBehavior)
            → Request-level (IRequestPipelineBehavior)
            → Type-specific (ICommandPipelineBehavior / IQueryPipelineBehavior / IEventPipelineBehavior)
            → Pre-handlers (IPreRequestHandler / IPreEventHandler)
            → Main handler (ICommandHandler / IQueryHandler / IEventHandler)
            → Post-handlers (IPostRequestHandler / IPostEventHandler)
```

Dispatchers use a `ConcurrentDictionary` to cache resolved handler delegates for performance.

### Multi-targeting

All packages target `.NET 8.0`, `9.0`, and `10.0`. The CI workflow (`build-and-test.yml`) runs the full test matrix against all three SDK versions.

### Sample Application

`samples/CleanArchitecture/` demonstrates real-world usage across four layers (Domain, Application, Infrastructure, WebApi) and is the canonical reference for how consumers should wire up the library.