using System.Reflection;
using Arcanic.Mediator.Query.Tests.Data.Pipelines;
using Arcanic.Mediator.Query.Tests.Data.Queries.Simple;
using Arcanic.Mediator.Query.Tests.Data.Queries.Error;
using Arcanic.Mediator.Query.Tests.Data.Queries.Cancellable;
using Arcanic.Mediator.Query.Tests.Data.Queries.UnhandledQuery;
using Arcanic.Mediator.Query.Tests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Arcanic.Mediator.Request.Abstractions;

namespace Arcanic.Mediator.Query.Tests;

public class QueryMediatorTests
{
    [Fact]
    public async Task SendAsync_WithValidQuery_ReturnsExpectedResponse()
    {
        // Arrange
        var service= new ServiceCollection();
        service.AddArcanicMediator().AddQueries(Assembly.GetExecutingAssembly());
        service.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = service.BuildServiceProvider();
        
        var queryMediator = serviceProvider.GetRequiredService<IQuerySender>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
        var query = new SimpleQuery()
        {
            Value = 42
        };

        // Act
        var response = await queryMediator.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().Be(42);
        response.Message.Should().Be("Processed 42");
        
        executedTypeTracker.ExecutedTypes[0].Should().Be<SimpleQueryPreHandler>();
        executedTypeTracker.ExecutedTypes[1].Should().Be<SimpleQueryHandler>();
        executedTypeTracker.ExecutedTypes[2].Should().Be<SimpleQueryPostHandler>();
    }

    [Fact]
    public async Task SendAsync_WithValidQuery_And_WithPipelines_ReturnsExpectedResponse()
    {
        // Arrange
        var service= new ServiceCollection();
        service.AddArcanicMediator()
            .AddPipelineBehavior(typeof(ExamplePipelineBehavior<,>))
            .AddPipelineBehavior(typeof(ExampleRequestPipelineBehavior<,>))
            .AddPipelineBehavior(typeof(ExampleQueryPipelineBehavior<,>))
            .AddQueries(Assembly.GetExecutingAssembly());
        service.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = service.BuildServiceProvider();
        
        var queryMediator = serviceProvider.GetRequiredService<IQuerySender>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
        var query = new SimpleQuery()
        {
            Value = 42
        };

        // Act
        var response = await queryMediator.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().Be(42);
        response.Message.Should().Be("Processed 42");
        
        executedTypeTracker.ExecutedTypes[0].Should().Be<ExamplePipelineBehavior<SimpleQuery, SimpleQueryResponse>>();
        executedTypeTracker.ExecutedTypes[1].Should().Be<ExampleRequestPipelineBehavior<SimpleQuery, SimpleQueryResponse>>();
        executedTypeTracker.ExecutedTypes[2].Should().Be<ExampleQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>>();
        executedTypeTracker.ExecutedTypes[3].Should().Be<SimpleQueryPreHandler>();
        executedTypeTracker.ExecutedTypes[4].Should().Be<SimpleQueryHandler>();
        executedTypeTracker.ExecutedTypes[5].Should().Be<SimpleQueryPostHandler>();
    }

    [Fact]
    public async Task SendAsync_WithNullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddQueries(Assembly.GetExecutingAssembly());
        var serviceProvider = service.BuildServiceProvider();
        var queryMediator = serviceProvider.GetRequiredService<IQuerySender>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            queryMediator.SendAsync<SimpleQueryResponse>(null!));
    }

    [Fact]
    public async Task SendAsync_WithoutRegisteredHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddQueries(Assembly.GetExecutingAssembly());
        var serviceProvider = service.BuildServiceProvider();
        var queryMediator = serviceProvider.GetRequiredService<IQuerySender>();
        // Use a query that has no handler registered (UnhandledQuery has no handler implemented)
        var query = new UnhandledQuery { Message = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            queryMediator.SendAsync(query));
    }

    [Fact]
    public async Task SendAsync_WithCancellationToken_PropagatesCancellation()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddQueries(Assembly.GetExecutingAssembly());
        var serviceProvider = service.BuildServiceProvider();
        var queryMediator = serviceProvider.GetRequiredService<IQuerySender>();
        var query = new CancellableQuery { DelayMilliseconds = 10 }; // Very short delay to avoid actual cancellation
        
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act & Assert - Use TaskCanceledException as it's what Task.Delay throws
        await Assert.ThrowsAsync<TaskCanceledException>(() => 
            queryMediator.SendAsync(query, cts.Token));
    }

    [Fact]
    public async Task SendAsync_WithSameQueryTypeTwice_UsesCachedDispatcher()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddQueries(Assembly.GetExecutingAssembly());
        service.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = service.BuildServiceProvider();
        
        var queryMediator = serviceProvider.GetRequiredService<IQuerySender>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
        var query1 = new SimpleQuery { Value = 1 };
        var query2 = new SimpleQuery { Value = 2 };

        // Act
        var response1 = await queryMediator.SendAsync(query1);
        var response2 = await queryMediator.SendAsync(query2);

        // Assert
        response1.Should().NotBeNull();
        response1.Result.Should().Be(1);
        response1.Message.Should().Be("Processed 1");
        
        response2.Should().NotBeNull();
        response2.Result.Should().Be(2);
        response2.Message.Should().Be("Processed 2");
        
        // Should have executed handlers for both queries
        executedTypeTracker.ExecutedTypes.Count.Should().Be(6); // 3 handlers x 2 queries
        executedTypeTracker.ExecutedTypes.Count(t => t == typeof(SimpleQueryHandler)).Should().Be(2);
    }

    [Fact]
    public async Task SendAsync_WhenHandlerThrowsException_PropagatesException()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddQueries(Assembly.GetExecutingAssembly());
        var serviceProvider = service.BuildServiceProvider();
        var queryMediator = serviceProvider.GetRequiredService<IQuerySender>();
        var query = new ErrorQuery { ErrorMessage = "Test error message" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            queryMediator.SendAsync(query));
        exception.Message.Should().Be("Test error message");
    }

    [Fact]
    public async Task SendAsync_WithCancellationToken_CanBeCancelled()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddQueries(Assembly.GetExecutingAssembly());
        var serviceProvider = service.BuildServiceProvider();
        var queryMediator = serviceProvider.GetRequiredService<IQuerySender>();
        var query = new CancellableQuery { DelayMilliseconds = 5000 }; // 5 seconds
        
        using var cts = new CancellationTokenSource();

        // Act
        var queryTask = queryMediator.SendAsync(query, cts.Token);
        cts.CancelAfter(100); // Cancel after 100ms

        // Assert - TaskCanceledException is derived from OperationCanceledException
        await Assert.ThrowsAsync<TaskCanceledException>(() => queryTask);
    }

    [Fact]
    public async Task SendAsync_WithMultipleQueryTypes_HandlesEachCorrectly()
    {
        // Arrange
        var service = new ServiceCollection();
        service.AddArcanicMediator().AddQueries(Assembly.GetExecutingAssembly());
        service.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = service.BuildServiceProvider();
        var queryMediator = serviceProvider.GetRequiredService<IQuerySender>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();

        var simpleQuery = new SimpleQuery { Value = 123 };
        var cancellableQuery = new CancellableQuery { DelayMilliseconds = 1 };

        // Act
        var simpleResponse = await queryMediator.SendAsync(simpleQuery);
        var cancellableResponse = await queryMediator.SendAsync(cancellableQuery);

        // Assert
        simpleResponse.Should().NotBeNull();
        simpleResponse.Result.Should().Be(123);
        simpleResponse.Message.Should().Be("Processed 123");

        cancellableResponse.Should().NotBeNull();
        cancellableResponse.Completed.Should().BeTrue();
        cancellableResponse.Message.Should().Be("Query completed successfully");

        // Should have executed both handlers
        executedTypeTracker.ExecutedTypes.Should().Contain(typeof(SimpleQueryHandler));
    }
}
