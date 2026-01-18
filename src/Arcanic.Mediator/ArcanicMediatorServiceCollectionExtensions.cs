using Arcanic.Mediator.Abstractions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register Arcanic Mediator services.
/// </summary>
public static class ArcanicMediatorServiceCollectionExtensions
{
    /// <summary>
    /// Adds Arcanic Mediator services to the dependency injection container with default configuration.
    /// </summary>
    /// <param name="services">The service collection to add the mediator services to.</param>
    /// <returns>A <see cref="DefaultArcanicMediatorBuilder"/> instance for further configuration.</returns>
    public static DefaultArcanicMediatorBuilder AddArcanicMediator(this IServiceCollection services)
    {
        return new DefaultArcanicMediatorBuilder(services, new  ArcanicMediatorServiceConfiguration());
    }

    /// <summary>
    /// Adds Arcanic Mediator services to the dependency injection container with custom configuration.
    /// </summary>
    /// <param name="services">The service collection to add the mediator services to.</param>
    /// <param name="configuration">An action to configure the mediator service options.</param>
    /// <returns>A <see cref="DefaultArcanicMediatorBuilder"/> instance for further configuration.</returns>
    public static DefaultArcanicMediatorBuilder AddArcanicMediator(this IServiceCollection services, Action<ArcanicMediatorServiceConfiguration> configuration)
    {
        var serviceConfig = new ArcanicMediatorServiceConfiguration();

        configuration.Invoke(serviceConfig);

        return services.AddArcanicMediator(serviceConfig);
    }

    /// <summary>
    /// Adds Arcanic Mediator services to the dependency injection container with the specified configuration instance.
    /// </summary>
    /// <param name="services">The service collection to add the mediator services to.</param>
    /// <param name="configuration">The mediator service configuration instance to use.</param>
    /// <returns>A <see cref="DefaultArcanicMediatorBuilder"/> instance for further configuration.</returns>
    public static DefaultArcanicMediatorBuilder AddArcanicMediator(this IServiceCollection services, ArcanicMediatorServiceConfiguration configuration)
    {
        return new DefaultArcanicMediatorBuilder(services, configuration);
    }
}
