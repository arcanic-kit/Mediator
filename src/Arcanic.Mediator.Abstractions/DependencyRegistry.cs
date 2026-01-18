using Arcanic.Mediator.Abstractions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Abstractions;

/// <summary>
/// Provides a common registry for dependency injection services that respects the Arcanic Mediator instance lifetime configuration.
/// This class centralizes service registration logic and ensures consistent lifetime management across all modules.
/// </summary>
public class DependencyRegistry
{
    /// <summary>
    /// The configuration settings for the Arcanic Mediator service, including service lifetime options.
    /// </summary>
    private readonly ArcanicMediatorServiceConfiguration _configuration;

    /// <summary>
    /// The service collection used for dependency injection registration.
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyRegistry"/> class.
    /// </summary>
    /// <param name="services">The service collection used for dependency injection registration.</param>
    /// <param name="configuration">The configuration settings for the Arcanic Mediator service.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    public DependencyRegistry(IServiceCollection services, ArcanicMediatorServiceConfiguration configuration)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Adds a service descriptor to the service collection using the configured instance lifetime.
    /// </summary>
    /// <param name="serviceType">The type of the service to register.</param>
    /// <param name="implementationType">The type that implements the service.</param>
    /// <returns>The current <see cref="DependencyRegistry"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceType"/> or <paramref name="implementationType"/> is null.</exception>
    public DependencyRegistry Add(Type serviceType, Type implementationType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(implementationType);

        _services.Add(new ServiceDescriptor(serviceType, implementationType, ConvertToServiceLifetime(_configuration.InstanceLifetime)));
        return this;
    }
    
    /// <summary>
    /// Converts the Arcanic Mediator InstanceLifetime to the corresponding Microsoft DI ServiceLifetime.
    /// </summary>
    /// <param name="instanceLifetime">The InstanceLifetime to convert.</param>
    /// <returns>The corresponding ServiceLifetime.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the InstanceLifetime value is not supported.</exception>
    private static ServiceLifetime ConvertToServiceLifetime(InstanceLifetime instanceLifetime)
    {
        return instanceLifetime switch
        {
            InstanceLifetime.Singleton => ServiceLifetime.Singleton,
            InstanceLifetime.Scoped => ServiceLifetime.Scoped,
            InstanceLifetime.Transient => ServiceLifetime.Transient,
            _ => throw new ArgumentOutOfRangeException(
                nameof(instanceLifetime),
                instanceLifetime,
                "Unknown InstanceLifetime value.")
        };
    }
}
