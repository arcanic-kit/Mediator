// using Arcanic.Mediator.Query.Abstractions;
// using Arcanic.Mediator.Query.Abstractions.Pipeline;
// using Arcanic.Mediator.Query.Tests.Core;
// using Arcanic.Mediator.Query.Tests.Data;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Arcanic.Mediator.Query.Tests;
//
// public class QueryPipelineBehaviorTests
// {
//     [Fact]
//     public async Task CustomPipelineBehavior_ExecutesInPipeline()
//     {
//         // Arrange
//         var services = ServiceCollectionCreator.CreateTestServiceCollection();
//         var behavior = new TestQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>();
//         services.AddScoped(typeof(IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>), _ => behavior);
//         var serviceProvider = services.BuildServiceProvider();
//
//         var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
//         var query = new SimpleQuery { Value = 5 };
//
//         // Act
//         var response = await mediator.SendAsync(query);
//
//         // Assert
//         Assert.True(behavior.WasExecuted);
//         Assert.NotNull(behavior.ReceivedQuery);
//         Assert.Equal(5, behavior.ReceivedQuery.Value);
//         Assert.NotNull(response);
//     }
//
//     [Fact]
//     public async Task MultiplePipelineBehaviors_ExecuteInCorrectOrder()
//     {
//         // Arrange
//         var executionOrder = new List<string>();
//         var services = ServiceCollectionCreator.CreateTestServiceCollection();
//         
//         var behavior1 = new OrderTrackingPipelineBehavior<SimpleQuery, SimpleQueryResponse>(executionOrder) { Name = "First" };
//         var behavior2 = new OrderTrackingPipelineBehavior<SimpleQuery, SimpleQueryResponse>(executionOrder) { Name = "Second" };
//         var behavior3 = new OrderTrackingPipelineBehavior<SimpleQuery, SimpleQueryResponse>(executionOrder) { Name = "Third" };
//
//         services.AddScoped(typeof(IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>), _ => behavior1);
//         services.AddScoped(typeof(IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>), _ => behavior2);
//         services.AddScoped(typeof(IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>), _ => behavior3);
//
//         var serviceProvider = services.BuildServiceProvider();
//         var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
//         var query = new SimpleQuery { Value = 10 };
//
//         // Act
//         await mediator.SendAsync(query);
//
//         // Assert
//         // Pipeline behaviors execute in reverse order of registration
//         // Each behavior wraps the next, so execution is: First_Before -> Second_Before -> Third_Before -> Handler -> Third_After -> Second_After -> First_After
//         Assert.Contains("PipelineBehavior_First_Before", executionOrder);
//         Assert.Contains("PipelineBehavior_Second_Before", executionOrder);
//         Assert.Contains("PipelineBehavior_Third_Before", executionOrder);
//         Assert.Contains("PipelineBehavior_First_After", executionOrder);
//         Assert.Contains("PipelineBehavior_Second_After", executionOrder);
//         Assert.Contains("PipelineBehavior_Third_After", executionOrder);
//     }
//
//     [Fact]
//     public async Task PipelineBehavior_CanModifyResponse()
//     {
//         // Arrange
//         var services = ServiceCollectionCreator.CreateTestServiceCollection();
//         var behavior = new ResponseModifyingPipelineBehavior { Modification = "CustomMod" };
//         services.AddScoped(typeof(IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>), _ => behavior);
//         var serviceProvider = services.BuildServiceProvider();
//
//         var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
//         var query = new SimpleQuery { Value = 5 };
//
//         // Act
//         var response = await mediator.SendAsync(query);
//
//         // Assert
//         Assert.NotNull(response);
//         Assert.Equal(10, response.Result);
//         Assert.Contains("CustomMod", response.Message);
//     }
//
//     [Fact]
//     public async Task PipelineBehavior_CanShortCircuitPipeline()
//     {
//         // Arrange
//         var services = ServiceCollectionCreator.CreateTestServiceCollection();
//         var shortCircuitResponse = new SimpleQueryResponse { Result = 999, Message = "Short-circuited" };
//         var behavior = new ShortCircuitPipelineBehavior { ShortCircuitResponse = shortCircuitResponse };
//         services.AddScoped(typeof(IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>), _ => behavior);
//         var serviceProvider = services.BuildServiceProvider();
//
//         var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
//         var query = new SimpleQuery { Value = 5 };
//
//         // Act
//         var response = await mediator.SendAsync(query);
//
//         // Assert
//         Assert.NotNull(response);
//         Assert.Equal(999, response.Result);
//         Assert.Equal("Short-circuited", response.Message);
//     }
//
//     [Fact]
//     public async Task PipelineBehavior_WithLogging_LogsExecution()
//     {
//         // Arrange
//         var logEntries = new List<string>();
//         var services = ServiceCollectionCreator.CreateTestServiceCollection();
//         var behavior = new LoggingPipelineBehavior<SimpleQuery, SimpleQueryResponse>(logEntries);
//         services.AddScoped(typeof(IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>), _ => behavior);
//         var serviceProvider = services.BuildServiceProvider();
//
//         var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
//         var query = new SimpleQuery { Value = 5 };
//
//         // Act
//         await mediator.SendAsync(query);
//
//         // Assert
//         Assert.Contains("Before: SimpleQuery", logEntries);
//         Assert.Contains("After: SimpleQuery", logEntries);
//     }
//
//     [Fact]
//     public async Task PipelineBehavior_ThrowsException_PropagatesException()
//     {
//         // Arrange
//         var services = ServiceCollectionCreator.CreateTestServiceCollection();
//         var behavior = new ExceptionThrowingPipelineBehavior<SimpleQuery, SimpleQueryResponse> { ErrorMessage = "Pipeline error" };
//         services.AddScoped(typeof(IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>), _ => behavior);
//         var serviceProvider = services.BuildServiceProvider();
//
//         var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
//         var query = new SimpleQuery { Value = 5 };
//
//         // Act & Assert
//         var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
//             await mediator.SendAsync(query));
//
//         Assert.Equal("Pipeline error", exception.Message);
//     }
// }
