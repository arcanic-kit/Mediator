using System.Reflection;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Tests.Data.Pipelines;
using Arcanic.Mediator.Query.Tests.Data.Queries.Simple;
using Arcanic.Mediator.Query.Tests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

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
        
        var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
        var query = new SimpleQuery()
        {
            Value = 42
        };

        // Act
        var response = await mediator.SendAsync(query);

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
        
        var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
        var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
        var query = new SimpleQuery();

        // Act
        var response = await mediator.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().Be(100);
        response.Message.Should().Be("Processed 100");
        
        executedTypeTracker.ExecutedTypes[0].Should().Be<ExamplePipelineBehavior<SimpleQuery, SimpleQueryResponse>>();
        executedTypeTracker.ExecutedTypes[1].Should().Be<ExampleRequestPipelineBehavior<SimpleQuery, SimpleQueryResponse>>();
        executedTypeTracker.ExecutedTypes[2].Should().Be<ExampleQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>>();
        executedTypeTracker.ExecutedTypes[3].Should().Be<SimpleQueryPreHandler>();
        executedTypeTracker.ExecutedTypes[4].Should().Be<SimpleQueryHandler>();
        executedTypeTracker.ExecutedTypes[5].Should().Be<SimpleQueryPostHandler>();
    }
}
