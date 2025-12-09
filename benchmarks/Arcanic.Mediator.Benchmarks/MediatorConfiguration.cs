using Arcanic.Mediator.Command;
using Arcanic.Mediator.Event;
using Arcanic.Mediator.Query;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Benchmarks;

/// <summary>
/// Configuration class for Arcanic Mediator services
/// </summary>
public static class MediatorConfiguration
{
    /// <summary>
    /// Registers Arcanic Mediator services with dependency injection
    /// </summary>
    public static IServiceCollection AddArcanicMediator(this IServiceCollection services)
    {
        services.AddArcanicMediator(moduleRegistry =>
        {
            // Register Command Module
            moduleRegistry.AddCommandModule(commandModuleBuilder =>
            {
                commandModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            });

            // Register Query Module
            moduleRegistry.AddQueryModule(queryModuleBuilder =>
            {
                queryModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            });

            // Register Event Module
            moduleRegistry.AddEventModule(eventModuleBuilder =>
            {
                eventModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            });
        });

        return services;
    }
}