using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Event.Benchmarks;

/// <summary>
/// Configuration class for Arcanic Event Mediator services
/// </summary>
public static class EventPublisherConfiguration
{
    /// <summary>
    /// Registers Arcanic Event Mediator services with dependency injection
    /// </summary>
    public static IServiceCollection AddArcanicEventPublisher(this IServiceCollection services)
    {
        services.AddArcanicMediator().AddEvents(Assembly.GetExecutingAssembly());
        return services;
    }
}
