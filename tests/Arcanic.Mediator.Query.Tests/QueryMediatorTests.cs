using System.Reflection;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Tests.Data.Queries.Simple;
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
        var serviceProvider = service.BuildServiceProvider();
        
        var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
        var query = new SimpleQuery();

        // Act
        var response = await mediator.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response.Result.Should().Be(100);
        response.Message.Should().Be("Processed 100");
        
        query.ExecutedTypes[0].Should().Be<SimpleQueryPreHandler>();
        query.ExecutedTypes[1].Should().Be<SimpleQueryHandler>();
        query.ExecutedTypes[2].Should().Be<SimpleQueryPostHandler>();
    }

    // [Fact]
    // public async Task SendAsync_WithNullQuery_ThrowsArgumentNullException()
    // {
    //     // Arrange
    //     var mediator = QueryMediatorCreator.Create();
    //     SimpleQuery? query = null;
    //
    //     // Act & Assert
    //     await Assert.ThrowsAsync<ArgumentNullException>(async () =>
    //         await mediator.SendAsync(query!));
    // }

    // [Fact]
    // public async Task SendAsync_WithCancellationToken_RespectsCancellation()
    // {
    //     // Arrange
    //     var mediator = QueryMediatorCreator.Create();
    //     var query = new AsyncQuery { DelayMs = 1000 };
    //     using var cts = new CancellationTokenSource();
    //     cts.CancelAfter(100);
    //
    //     // Act & Assert
    //     await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
    //         await mediator.SendAsync(query, cts.Token));
    // }
    //
    // [Fact]
    // public async Task SendAsync_WithComplexQuery_ReturnsComplexResponse()
    // {
    //     // Arrange
    //     var mediator = QueryMediatorCreator.Create();
    //     var query = new ComplexQuery { Name = "Item", Count = 3 };
    //
    //     // Act
    //     var response = await mediator.SendAsync(query);
    //
    //     // Assert
    //     Assert.NotNull(response);
    //     Assert.Equal(3, response.Items.Count);
    //     Assert.Equal(3, response.TotalCount);
    //     Assert.Equal("Item_0", response.Items[0].Name);
    //     Assert.Equal(0, response.Items[0].Index);
    // }
    //
    // [Fact]
    // public async Task SendAsync_WithAsyncQuery_CompletesAsynchronously()
    // {
    //     // Arrange
    //     var mediator = QueryMediatorCreator.Create();
    //     var query = new AsyncQuery { DelayMs = 50 };
    //     var startTime = DateTime.UtcNow;
    //
    //     // Act
    //     var response = await mediator.SendAsync(query);
    //     var endTime = DateTime.UtcNow;
    //
    //     // Assert
    //     Assert.NotNull(response);
    //     Assert.True((endTime - startTime).TotalMilliseconds >= 50);
    //     Assert.Equal(50, response.DelayMs);
    // }
}
