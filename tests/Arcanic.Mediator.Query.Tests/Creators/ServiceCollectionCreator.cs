using System.Reflection;
using Arcanic.Mediator.Abstractions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Query.Tests.Creators;

/// <summary>
/// Extension methods for setting up test service collections with query mediator configuration.
/// </summary>
public static class ServiceCollectionCreator
{
    /// <summary>
    /// Creates a service collection with query mediator configured with default settings.
    /// </summary>
    public static IServiceCollection CreateTestServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator().AddQueries(Assembly.GetExecutingAssembly());
        return services;
    }

    /// <summary>
    /// Creates a service collection with query mediator configured with specific instance lifetime.
    /// </summary>
    public static IServiceCollection CreateTestServiceCollection(InstanceLifetime lifetime)
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator(config => config.InstanceLifetime = lifetime)
            .AddQueries(Assembly.GetExecutingAssembly());
        return services;
    }

    /// <summary>
    /// Creates a service collection with query mediator and registers a specific query handler.
    /// </summary>
    public static IServiceCollection CreateTestServiceCollection<TQuery, TResponse, THandler>()
        where TQuery : Arcanic.Mediator.Query.Abstractions.IQuery<TResponse>
        where THandler : class, Arcanic.Mediator.Query.Abstractions.Handler.IQueryHandler<TQuery, TResponse>
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator()
            .AddQueries(Assembly.GetExecutingAssembly());
        services.AddScoped<Arcanic.Mediator.Query.Abstractions.Handler.IQueryHandler<TQuery, TResponse>, THandler>();
        return services;
    }

    /// <summary>
    /// Creates a service collection with query mediator and registers handlers from a specific assembly.
    /// </summary>
    public static IServiceCollection CreateTestServiceCollection(Assembly assembly)
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator().AddQueries(assembly);
        return services;
    }
}
