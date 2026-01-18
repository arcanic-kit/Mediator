using Arcanic.Mediator.Abstractions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Abstractions;

/// <summary>
/// Provides lazy singleton access to a DependencyRegistry instance.
/// This accessor ensures that only one DependencyRegistry instance is created per accessor instance,
/// improving performance and consistency when multiple service registrations are performed.
/// </summary>
public class DependencyRegistryAccessor
{
    /// <summary>
    /// The lazy-initialized DependencyRegistry instance.
    /// </summary>
    private static Lazy<DependencyRegistry>? _lazyRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyRegistryAccessor"/> class.
    /// </summary>
    /// <param name="services">The service collection used for dependency injection registration.</param>
    /// <param name="configuration">The configuration settings for the Arcanic Mediator service.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    public DependencyRegistryAccessor(IServiceCollection services, ArcanicMediatorServiceConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        if (_lazyRegistry is null || !_lazyRegistry.IsValueCreated)
        {
            _lazyRegistry = new(() => new DependencyRegistry(services, configuration), LazyThreadSafetyMode.ExecutionAndPublication); 
        }
    }

    /// <summary>
    /// Gets the singleton DependencyRegistry instance.
    /// The registry is created lazily on first access and reused for subsequent calls.
    /// </summary>
    /// <value>The <see cref="DependencyRegistry"/> singleton instance.</value>
    public DependencyRegistry Registry => _lazyRegistry!.Value;
}
