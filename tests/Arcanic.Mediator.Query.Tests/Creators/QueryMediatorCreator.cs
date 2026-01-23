using Arcanic.Mediator.Abstractions.Configuration;
using Arcanic.Mediator.Query.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Query.Tests.Creators;

/// <summary>
/// Factory class for creating IQueryMediator instances with various configurations for testing.
/// </summary>
public static class QueryMediatorCreator
{
    /// <summary>
    /// Creates a query mediator with default configuration.
    /// </summary>
    public static IQueryMediator Create()
    {
        var services = ServiceCollectionCreator.CreateTestServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IQueryMediator>();
    }

    /// <summary>
    /// Creates a query mediator with specific instance lifetime.
    /// </summary>
    public static IQueryMediator Create(InstanceLifetime lifetime)
    {
        var services = ServiceCollectionCreator.CreateTestServiceCollection(lifetime);
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IQueryMediator>();
    }

    /// <summary>
    /// Creates a query mediator from a custom service collection.
    /// </summary>
    public static IQueryMediator Create(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IQueryMediator>();
    }

    /// <summary>
    /// Creates a query mediator from a service provider.
    /// </summary>
    public static IQueryMediator Create(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IQueryMediator>();
    }

    /// <summary>
    /// Creates a scoped query mediator (useful for testing scoped lifetime).
    /// </summary>
    public static (IQueryMediator Mediator, IServiceScope Scope) CreateScoped()
    {
        var services = ServiceCollectionCreator.CreateTestServiceCollection(InstanceLifetime.Scoped);
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IQueryMediator>();
        return (mediator, scope);
    }
}
