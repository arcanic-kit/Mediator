using Arcanic.Mediator.Command.Tests.Data.Commands.Cancellable;
using Arcanic.Mediator.Command.Tests.Data.Commands.Error;
using Arcanic.Mediator.Command.Tests.Data.Commands.Simple;
using Arcanic.Mediator.Command.Tests.Data.Commands.UnhandledCommand;
using Arcanic.Mediator.Command.Tests.Data.Commands.VoidCommand;
using Arcanic.Mediator.Command.Tests.Data.Pipelines;
using Arcanic.Mediator.Command.Tests.Utils;
using Arcanic.Mediator.Request.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Command.Tests;

public class CommandMediatorTests
{
    [Fact]
    public async Task SendAsync_WithValidCommandWithResponse_ReturnsExpectedResponse()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        service.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = service.BuildServiceProvider();
        
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
        var command = new SimpleCommand
        {
            Value = 42
        };

        // Act
        var response = await mediator.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().Be(42);
        response.Message.Should().Be("Processed 42");
        
        executedTypeTracker.ExecutedTypes[0].Should().Be<SimpleCommandPreHandler>();
        executedTypeTracker.ExecutedTypes[1].Should().Be<SimpleCommandHandler>();
        executedTypeTracker.ExecutedTypes[2].Should().Be<SimpleCommandPostHandler>();
    }

    [Fact]
    public async Task SendAsync_WithValidVoidCommand_CompletesSuccessfully()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        service.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = service.BuildServiceProvider();
        
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
        var command = new VoidCommand
        {
            Message = "Test void command"
        };

        // Act
        await mediator.SendAsync(command);

        // Assert
        executedTypeTracker.ExecutedTypes.Should().ContainSingle()
            .Which.Should().Be<VoidCommandHandler>();
    }

    [Fact]
    public async Task SendAsync_WithValidCommandWithResponse_And_WithPipelines_ReturnsExpectedResponse()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator()
            .AddPipelineBehavior(typeof(ExamplePipelineBehavior<,>))
            .AddPipelineBehavior(typeof(ExampleRequestPipelineBehavior<,>))
            .AddPipelineBehavior(typeof(ExampleCommandPipelineBehavior<,>))
            .AddCommands(Assembly.GetExecutingAssembly());
        service.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = service.BuildServiceProvider();
        
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
        var command = new SimpleCommand
        {
            Value = 42
        };

        // Act
        var response = await mediator.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().Be(42);
        response.Message.Should().Be("Processed 42");
        
        executedTypeTracker.ExecutedTypes[0].Should().Be<ExamplePipelineBehavior<SimpleCommand, SimpleCommandResponse>>();
        executedTypeTracker.ExecutedTypes[1].Should().Be<ExampleRequestPipelineBehavior<SimpleCommand, SimpleCommandResponse>>();
        executedTypeTracker.ExecutedTypes[2].Should().Be<ExampleCommandPipelineBehavior<SimpleCommand, SimpleCommandResponse>>();
        executedTypeTracker.ExecutedTypes[3].Should().Be<SimpleCommandPreHandler>();
        executedTypeTracker.ExecutedTypes[4].Should().Be<SimpleCommandHandler>();
        executedTypeTracker.ExecutedTypes[5].Should().Be<SimpleCommandPostHandler>();
    }

    [Fact]
    public async Task SendAsync_WithNullCommandWithResponse_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        var serviceProvider = service.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            mediator.SendAsync<SimpleCommandResponse>(null!));
    }

    [Fact]
    public async Task SendAsync_WithNullVoidCommand_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        var serviceProvider = service.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            mediator.SendAsync(null!));
    }

    [Fact]
    public async Task SendAsync_WithoutRegisteredHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        var serviceProvider = service.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        // Use a command that has no handler registered (UnhandledCommand has no handler implemented)
        var command = new UnhandledCommand { Message = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            mediator.SendAsync(command));
    }

    [Fact]
    public async Task SendAsync_WithCancellationToken_PropagatesCancellation()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        var serviceProvider = service.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var command = new CancellableCommand { DelayMilliseconds = 10 }; // Very short delay to avoid actual cancellation
        
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act & Assert - Use TaskCanceledException as it's what Task.Delay throws
        await Assert.ThrowsAsync<TaskCanceledException>(() => 
            mediator.SendAsync(command, cts.Token));
    }

    [Fact]
    public async Task SendAsync_WithSameCommandTypeTwice_UsesCachedDispatcher()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        service.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = service.BuildServiceProvider();
        
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
        var command1 = new SimpleCommand { Value = 1 };
        var command2 = new SimpleCommand { Value = 2 };

        // Act
        var response1 = await mediator.SendAsync(command1);
        var response2 = await mediator.SendAsync(command2);

        // Assert
        response1.Should().NotBeNull();
        response1.Result.Should().Be(1);
        response1.Message.Should().Be("Processed 1");
        
        response2.Should().NotBeNull();
        response2.Result.Should().Be(2);
        response2.Message.Should().Be("Processed 2");
        
        // Should have executed handlers for both commands
        executedTypeTracker.ExecutedTypes.Count.Should().Be(6); // 3 handlers x 2 commands
        executedTypeTracker.ExecutedTypes.Count(t => t == typeof(SimpleCommandHandler)).Should().Be(2);
    }

    [Fact]
    public async Task SendAsync_WhenHandlerThrowsException_PropagatesException()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        var serviceProvider = service.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var command = new ErrorCommand { ErrorMessage = "Test error message" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            mediator.SendAsync(command));
        exception.Message.Should().Be("Test error message");
    }

    [Fact]
    public async Task SendAsync_WithCancellationToken_CanBeCancelled()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        var serviceProvider = service.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var command = new CancellableCommand { DelayMilliseconds = 5000 }; // 5 seconds
        
        using var cts = new CancellationTokenSource();

        // Act
        var commandTask = mediator.SendAsync(command, cts.Token);
        cts.CancelAfter(100); // Cancel after 100ms

        // Assert - TaskCanceledException is derived from OperationCanceledException
        await Assert.ThrowsAsync<TaskCanceledException>(() => commandTask);
    }

    [Fact]
    public async Task SendAsync_WithMultipleCommandTypes_HandlesEachCorrectly()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        service.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = service.BuildServiceProvider();
        
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
        var simpleCommand = new SimpleCommand { Value = 99 };
        var voidCommand = new VoidCommand { Message = "Test void" };

        // Act
        var simpleResponse = await mediator.SendAsync(simpleCommand);
        await mediator.SendAsync(voidCommand);

        // Assert
        simpleResponse.Should().NotBeNull();
        simpleResponse.Result.Should().Be(99);
        simpleResponse.Message.Should().Be("Processed 99");
        
        // Should have executed handlers for both command types
        executedTypeTracker.ExecutedTypes.Should().Contain(typeof(SimpleCommandHandler));
        executedTypeTracker.ExecutedTypes.Should().Contain(typeof(VoidCommandHandler));
        executedTypeTracker.ExecutedTypes.Count.Should().Be(4); // 3 for simple + 1 for void
    }

    [Fact]
    public async Task SendAsync_WithPreAndPostHandlers_ExecutesInCorrectOrder()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        service.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = service.BuildServiceProvider();
        
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
        var command = new SimpleCommand { Value = 123 };

        // Act
        var response = await mediator.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().Be(123);
        
        // Verify execution order: Pre -> Main -> Post
        executedTypeTracker.ExecutedTypes.Should().HaveCount(3);
        executedTypeTracker.ExecutedTypes[0].Should().Be<SimpleCommandPreHandler>();
        executedTypeTracker.ExecutedTypes[1].Should().Be<SimpleCommandHandler>();
        executedTypeTracker.ExecutedTypes[2].Should().Be<SimpleCommandPostHandler>();
    }

    [Fact]
    public async Task SendAsync_WithPipelineBehaviors_ExecutesInCorrectOrder()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator()
            .AddPipelineBehavior(typeof(ExamplePipelineBehavior<,>))
            .AddPipelineBehavior(typeof(ExampleRequestPipelineBehavior<,>))
            .AddPipelineBehavior(typeof(ExampleCommandPipelineBehavior<,>))
            .AddCommands(Assembly.GetExecutingAssembly());
        service.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = service.BuildServiceProvider();
        
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
        var command = new SimpleCommand { Value = 456 };

        // Act
        var response = await mediator.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().Be(456);
        
        // Verify execution order: Pipelines (in registration order) -> Pre -> Main -> Post
        executedTypeTracker.ExecutedTypes.Should().HaveCount(6);
        executedTypeTracker.ExecutedTypes[0].Should().Be<ExamplePipelineBehavior<SimpleCommand, SimpleCommandResponse>>();
        executedTypeTracker.ExecutedTypes[1].Should().Be<ExampleRequestPipelineBehavior<SimpleCommand, SimpleCommandResponse>>();
        executedTypeTracker.ExecutedTypes[2].Should().Be<ExampleCommandPipelineBehavior<SimpleCommand, SimpleCommandResponse>>();
        executedTypeTracker.ExecutedTypes[3].Should().Be<SimpleCommandPreHandler>();
        executedTypeTracker.ExecutedTypes[4].Should().Be<SimpleCommandHandler>();
        executedTypeTracker.ExecutedTypes[5].Should().Be<SimpleCommandPostHandler>();
    }
}
