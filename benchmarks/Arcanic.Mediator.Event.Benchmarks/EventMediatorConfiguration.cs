using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Event.Benchmarks;

/// <summary>
/// Configuration class for Arcanic Event Mediator services
/// </summary>
public static class EventMediatorConfiguration
{
    /// <summary>
    /// Registers Arcanic Event Mediator services with dependency injection
    /// </summary>
    public static IServiceCollection AddArcanicEventMediator(this IServiceCollection services)
    {
        services.AddArcanicMediator(moduleRegistry =>
        {
            // Register Event Module
            moduleRegistry.AddEventModule(eventModuleBuilder =>
            {
                eventModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            });
        });

        return services;
    }
}
