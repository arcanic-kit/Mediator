// using Arcanic.Mediator.Query.Abstractions;
// using Arcanic.Mediator.Query.Abstractions.Handler;
// using Arcanic.Mediator.Query.Abstractions.Pipeline;
// using Arcanic.Mediator.Query.Tests.Core;
// using Arcanic.Mediator.Query.Tests.Data;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Arcanic.Mediator.Query.Tests;
//
// public class IntegrationScenariosTests
// {
//     //[Fact]
//     //public async Task FullPipeline_WithPrePostAndPipelineBehaviors_ExecutesInOrder()
//     //{
//     //    // Arrange
//     //    var executionOrder = new List<string>();
//     //    var services = ServiceCollectionCreator.CreateTestServiceCollection();
//         
//     //    // Add pre-handlers
//     //    var preHandler1 = new OrderTrackingPreHandler(executionOrder) { Name = "Pre1" };
//     //    var preHandler2 = new OrderTrackingPreHandler(executionOrder) { Name = "Pre2" };
//     //    services.AddScoped<IQueryPreHandler<SimpleQuery>>(_ => preHandler1);
//     //    services.AddScoped<IQueryPreHandler<SimpleQuery>>(_ => preHandler2);
//
//     //    // Add pipeline behaviors
//     //    var behavior1 = new OrderTrackingPipelineBehavior<SimpleQuery, SimpleQueryResponse>(executionOrder) { Name = "Behavior1" };
//     //    var behavior2 = new OrderTrackingPipelineBehavior<SimpleQuery, SimpleQueryResponse>(executionOrder) { Name = "Behavior2" };
//     //    services.AddScoped(typeof(IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>), _ => behavior1);
//     //    services.AddScoped(typeof(IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>), _ => behavior2);
//
//     //    // Add post-handlers
//     //    var postHandler1 = new OrderTrackingPostHandler(executionOrder) { Name = "Post1" };
//     //    var postHandler2 = new OrderTrackingPostHandler(executionOrder) { Name = "Post2" };
//     //    services.AddScoped<IQueryPostHandler<SimpleQuery>>(_ => postHandler1);
//     //    services.AddScoped<IQueryPostHandler<SimpleQuery>>(_ => postHandler2);
//
//     //    var serviceProvider = services.BuildServiceProvider();
//     //    var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
//     //    var query = new SimpleQuery { Value = 10 };
//
//     //    // Act
//     //    var response = await mediator.SendAsync(query);
//
//     //    // Assert
//     //    Assert.NotNull(response);
//     //    // Verify execution order: Pre-handlers -> Pipeline behaviors (before) -> Handler -> Pipeline behaviors (after) -> Post-handlers
//     //    Assert.Contains("PreHandler_Pre1", executionOrder);
//     //    Assert.Contains("PreHandler_Pre2", executionOrder);
//     //    Assert.Contains("PipelineBehavior_Behavior1_Before", executionOrder);
//     //    Assert.Contains("PipelineBehavior_Behavior2_Before", executionOrder);
//     //    Assert.Contains("PipelineBehavior_Behavior2_After", executionOrder);
//     //    Assert.Contains("PipelineBehavior_Behavior1_After", executionOrder);
//     //    Assert.Contains("PostHandler_Post1", executionOrder);
//     //    Assert.Contains("PostHandler_Post2", executionOrder);
//     //}
//
//     [Fact]
//     public async Task MultipleQueries_WithDifferentHandlers_ExecuteIndependently()
//     {
//         // Arrange
//         var mediator = QueryMediatorCreator.Create();
//         var simpleQuery = new SimpleQuery { Value = 5 };
//         var complexQuery = new ComplexQuery { Name = "Item", Count = 3 };
//         var asyncQuery = new AsyncQuery { DelayMs = 10 };
//
//         // Act
//         var simpleResponse = await mediator.SendAsync(simpleQuery);
//         var complexResponse = await mediator.SendAsync(complexQuery);
//         var asyncResponse = await mediator.SendAsync(asyncQuery);
//
//         // Assert
//         Assert.NotNull(simpleResponse);
//         Assert.Equal(10, simpleResponse.Result);
//
//         Assert.NotNull(complexResponse);
//         Assert.Equal(3, complexResponse.Items.Count);
//
//         Assert.NotNull(asyncResponse);
//         Assert.Equal(10, asyncResponse.DelayMs);
//     }
//
//     [Fact]
//     public async Task QueryWithDependencies_ResolvesCorrectly()
//     {
//         // Arrange
//         var services = ServiceCollectionCreator.CreateTestServiceCollection();
//         services.AddScoped<ITestDependency, TestDependency>();
//         var serviceProvider = services.BuildServiceProvider();
//
//         var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
//         var query = new QueryWithDependencies { Data = "test-input" };
//
//         // Act
//         var response = await mediator.SendAsync(query);
//
//         // Assert
//         Assert.NotNull(response);
//         Assert.Equal("Processed: test-input", response.ProcessedData);
//         Assert.Equal("TestDependency", response.ServiceName);
//     }
//
//     [Fact]
//     public async Task ConcurrentQueries_ExecuteSuccessfully()
//     {
//         // Arrange
//         var mediator = QueryMediatorCreator.Create();
//         var queries = Enumerable.Range(1, 10)
//             .Select(i => new SimpleQuery { Value = i })
//             .ToList();
//
//         // Act
//         var tasks = queries.Select(q => mediator.SendAsync(q));
//         var responses = await Task.WhenAll(tasks);
//
//         // Assert
//         Assert.Equal(10, responses.Length);
//         for (int i = 0; i < responses.Length; i++)
//         {
//             Assert.NotNull(responses[i]);
//             Assert.Equal((i + 1) * 2, responses[i].Result);
//         }
//     }
//
//     [Fact]
//     public async Task QueryWithPreAndPostHandlers_ExecutesInCorrectOrder()
//     {
//         // Arrange
//         var services = ServiceCollectionCreator.CreateTestServiceCollection();
//         var preHandler = new TestPreHandler();
//         var postHandler = new TestPostHandler();
//         services.AddScoped<IQueryPreHandler<SimpleQuery>>(_ => preHandler);
//         services.AddScoped<IQueryPostHandler<SimpleQuery>>(_ => postHandler);
//         var serviceProvider = services.BuildServiceProvider();
//
//         var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
//         var query = new SimpleQuery { Value = 5 };
//
//         // Act
//         var response = await mediator.SendAsync(query);
//
//         // Assert
//         Assert.True(preHandler.WasExecuted);
//         Assert.True(postHandler.WasExecuted);
//         Assert.NotNull(response);
//     }
//
//     [Fact]
//     public async Task QueryWithPipelineBehavior_ModifiesResponse()
//     {
//         // Arrange
//         var services = ServiceCollectionCreator.CreateTestServiceCollection();
//         var behavior = new ResponseModifyingPipelineBehavior { Modification = "TestMod" };
//         services.AddScoped(typeof(IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>), _ => behavior);
//         var serviceProvider = services.BuildServiceProvider();
//
//         var mediator = serviceProvider.GetRequiredService<IQueryMediator>();
//         var query = new SimpleQuery { Value = 7 };
//
//         // Act
//         var response = await mediator.SendAsync(query);
//
//         // Assert
//         Assert.NotNull(response);
//         Assert.Contains("TestMod", response.Message);
//     }
//
//     [Fact]
//     public async Task MultipleQueriesWithSameType_ExecuteIndependently()
//     {
//         // Arrange
//         var mediator = QueryMediatorCreator.Create();
//         var query1 = new SimpleQuery { Value = 1 };
//         var query2 = new SimpleQuery { Value = 2 };
//         var query3 = new SimpleQuery { Value = 3 };
//
//         // Act
//         var response1 = await mediator.SendAsync(query1);
//         var response2 = await mediator.SendAsync(query2);
//         var response3 = await mediator.SendAsync(query3);
//
//         // Assert
//         Assert.Equal(2, response1.Result);
//         Assert.Equal(4, response2.Result);
//         Assert.Equal(6, response3.Result);
//     }
//
//     [Fact]
//     public async Task QueryWithException_PropagatesCorrectly()
//     {
//         // Arrange
//         var mediator = QueryMediatorCreator.Create();
//         var query = new ExceptionQuery { ErrorMessage = "Test error message" };
//
//         // Act & Assert
//         var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
//             await mediator.SendAsync(query));
//
//         Assert.Equal("Test error message", exception.Message);
//     }
// }
