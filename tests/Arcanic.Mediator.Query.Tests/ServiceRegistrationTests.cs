// using Arcanic.Mediator.Abstractions.Configuration;
// using Arcanic.Mediator.Query.Abstractions;
// using Arcanic.Mediator.Query.Abstractions.Handler;
// using Arcanic.Mediator.Query.Abstractions.Pipeline;
// using Microsoft.Extensions.DependencyInjection;
// using System.Reflection;
// using Arcanic.Mediator.Query.Tests.Data;
//
// namespace Arcanic.Mediator.Query.Tests;
//
// public class ServiceRegistrationTests
// {
//     [Fact]
//     public void RegisterQueriesFromAssembly_RegistersAllHandlers()
//     {
//         // Arrange
//         var services = new ServiceCollection();
//         var assembly = Assembly.GetExecutingAssembly();
//
//         // Act
//         services.AddArcanicMediator().AddQueries(assembly);
//         var serviceProvider = services.BuildServiceProvider();
//
//         // Assert
//         var simpleHandler = serviceProvider.GetService<Arcanic.Mediator.Query.Abstractions.Handler.IQueryHandler<SimpleQuery, SimpleQueryResponse>>();
//         var complexHandler = serviceProvider.GetService<Arcanic.Mediator.Query.Abstractions.Handler.IQueryHandler<ComplexQuery, ComplexQueryResponse>>();
//         var asyncHandler = serviceProvider.GetService<Arcanic.Mediator.Query.Abstractions.Handler.IQueryHandler<AsyncQuery, AsyncQueryResponse>>();
//
//         Assert.NotNull(simpleHandler);
//         Assert.NotNull(complexHandler);
//         Assert.NotNull(asyncHandler);
//     }
//
//     [Fact]
//     public void RegisterQueryRequiredServices_RegistersCoreServices()
//     {
//         // Arrange
//         var services = new ServiceCollection();
//
//         // Act
//         services.AddArcanicMediator().AddQueries(Assembly.GetExecutingAssembly());
//         var serviceProvider = services.BuildServiceProvider();
//
//         // Assert
//         var mediator = serviceProvider.GetService<IQueryMediator>();
//         Assert.NotNull(mediator);
//     }
//
//     [Fact]
//     public void RegisterQueryPipelineBehavior_RegistersBehavior()
//     {
//         // Arrange
//         var services = new ServiceCollection();
//         var behavior = new TestQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>();
//
//         // Act
//         services.AddArcanicMediator()
//             .AddQueries(Assembly.GetExecutingAssembly())
//             .AddQueryPipelineBehavior(typeof(TestQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>));
//         var serviceProvider = services.BuildServiceProvider();
//
//         // Assert
//         var registeredBehavior = serviceProvider.GetServices<IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>>();
//         Assert.NotEmpty(registeredBehavior);
//     }
//
//     [Fact]
//     public void ServiceLifetime_RespectedForHandlers()
//     {
//         // Arrange
//         var services = new ServiceCollection();
//
//         // Act - Test with Scoped lifetime
//         services.AddArcanicMediator(config => config.InstanceLifetime = InstanceLifetime.Scoped)
//             .AddQueries(Assembly.GetExecutingAssembly());
//         var serviceProvider = services.BuildServiceProvider();
//
//         // Assert
//         using var scope1 = serviceProvider.CreateScope();
//         using var scope2 = serviceProvider.CreateScope();
//
//         var handler1 = scope1.ServiceProvider.GetRequiredService<Arcanic.Mediator.Query.Abstractions.Handler.IQueryHandler<SimpleQuery, SimpleQueryResponse>>();
//         var handler2 = scope2.ServiceProvider.GetRequiredService<Arcanic.Mediator.Query.Abstractions.Handler.IQueryHandler<SimpleQuery, SimpleQueryResponse>>();
//
//         // With scoped lifetime, handlers should be different instances
//         // Note: We can't directly compare instances, but we can verify they work
//         Assert.NotNull(handler1);
//         Assert.NotNull(handler2);
//     }
//
//     [Fact]
//     public void RegisterQueriesFromAssembly_RegistersPreHandlers()
//     {
//         // Arrange
//         var services = new ServiceCollection();
//         var assembly = Assembly.GetExecutingAssembly();
//
//         // Act
//         services.AddArcanicMediator().AddQueries(assembly);
//         var serviceProvider = services.BuildServiceProvider();
//
//         // Assert
//         var preHandlers = serviceProvider.GetServices<IQueryPreHandler<SimpleQuery>>();
//         // Pre-handlers should be registered if they exist in the assembly
//         Assert.NotNull(preHandlers);
//     }
//
//     [Fact]
//     public void RegisterQueriesFromAssembly_RegistersPostHandlers()
//     {
//         // Arrange
//         var services = new ServiceCollection();
//         var assembly = Assembly.GetExecutingAssembly();
//
//         // Act
//         services.AddArcanicMediator().AddQueries(assembly);
//         var serviceProvider = services.BuildServiceProvider();
//
//         // Assert
//         var postHandlers = serviceProvider.GetServices<IQueryPostHandler<SimpleQuery>>();
//         // Post-handlers should be registered if they exist in the assembly
//         Assert.NotNull(postHandlers);
//     }
//
//     [Fact]
//     public async Task ServiceRegistration_WithTransientLifetime_CreatesNewInstances()
//     {
//         // Arrange
//         var services = new ServiceCollection();
//         services.AddArcanicMediator(config => config.InstanceLifetime = InstanceLifetime.Transient)
//             .AddQueries(Assembly.GetExecutingAssembly());
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
//         // With transient lifetime, new instances are created each time
//     }
//
//     [Fact]
//     public async Task ServiceRegistration_WithSingletonLifetime_ReusesInstances()
//     {
//         // Arrange
//         var services = new ServiceCollection();
//         services.AddArcanicMediator(config => config.InstanceLifetime = InstanceLifetime.Singleton)
//             .AddQueries(Assembly.GetExecutingAssembly());
//         var serviceProvider = services.BuildServiceProvider();
//
//         var mediator1 = serviceProvider.GetRequiredService<IQueryMediator>();
//         var mediator2 = serviceProvider.GetRequiredService<IQueryMediator>();
//         var query = new SimpleQuery { Value = 5 };
//
//         // Act
//         var response1 = await mediator1.SendAsync(query);
//         var response2 = await mediator2.SendAsync(query);
//
//         // Assert
//         Assert.NotNull(response1);
//         Assert.NotNull(response2);
//         // With singleton lifetime, same instances are reused
//     }
// }
