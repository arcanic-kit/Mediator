using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Abstractions.Modules;

/// <summary>
/// Defines a contract for modules that can configure dependency injection services.
/// Modules provide a way to encapsulate and organize service registration logic.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Configures the dependency injection container by registering services, implementations, and configurations.
    /// </summary>
    /// <param name="services">The service collection to configure with module-specific registrations.</param>
    void Build(IServiceCollection services);
}