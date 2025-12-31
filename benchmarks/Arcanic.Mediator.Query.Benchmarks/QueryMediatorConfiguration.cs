using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Query.Benchmarks;

/// <summary>
/// Configuration class for Arcanic Query Mediator services
/// </summary>
public static class QueryMediatorConfiguration
{
    /// <summary>
    /// Registers Arcanic Query Mediator services with dependency injection
    /// </summary>
    public static IServiceCollection AddArcanicQueryMediator(this IServiceCollection services)
    {
        services.AddArcanicMediator(moduleRegistry =>
        {
            // Register Query Module
            moduleRegistry.AddQueryModule(queryModuleBuilder =>
            {
                queryModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            });
        });

        return services;
    }
}
