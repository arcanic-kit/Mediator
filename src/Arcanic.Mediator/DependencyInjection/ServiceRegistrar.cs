using Arcanic.Mediator.Abstractions.Configuration;
using Arcanic.Mediator.Abstractions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.DependencyInjection;

/// <summary>
/// Provides service registration functionality for the Arcanic Mediator dependency injection container.
/// This class facilitates the registration of service types with their corresponding implementation types
/// using the configured instance lifetime from the mediator configuration.
/// </summary>
public class ServiceRegistrar : IServiceRegistrar
{
    /// <summary>
    /// The configuration settings for the Arcanic Mediator service, including service lifetime options.
    /// </summary>
    private readonly ArcanicMediatorConfiguration _configuration;

    /// <summary>
    /// The service collection used for dependency injection registration.
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRegistrar"/> class.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    /// <param name="configuration">The Arcanic Mediator configuration containing instance lifetime settings.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="configuration"/> is <c>null</c>.
    /// </exception>
    public ServiceRegistrar(ArcanicMediatorConfiguration configuration, IServiceCollection services)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }
    
    /// <summary>
    /// Registers a service type with its implementation type using the configured instance lifetime.
    /// </summary>
    /// <param name="serviceType">The type of service to register.</param>
    /// <param name="implementationType">The implementation type that provides the service.</param>
    /// <returns>The current <see cref="ServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="serviceType"/> or <paramref name="implementationType"/> is <c>null</c>.
    /// </exception>
    public IServiceRegistrar Register(Type serviceType, Type implementationType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(implementationType);

        _services.Add(new ServiceDescriptor(serviceType, implementationType, ConvertToServiceLifetime(_configuration.InstanceLifetime)));
        return this;
    }

    /// <summary>
    /// Gets a required service from the service collection by building a temporary service provider.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <returns>The service instance of type <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the service of type <typeparamref name="T"/> is not registered in the service collection.
    /// </exception>
    public T GetRequiredService<T>() where T : class
    {
        using var serviceProvider = _services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<T>();
    }
    
    /// <summary>
    /// Converts an <see cref="InstanceLifetime"/> enumeration value to the corresponding <see cref="ServiceLifetime"/> value.
    /// </summary>
    /// <param name="instanceLifetime">The instance lifetime to convert.</param>
    /// <returns>The corresponding <see cref="ServiceLifetime"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="instanceLifetime"/> contains an unknown or unsupported value.
    /// </exception>
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