using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Abstractions;

/// <summary>
/// Represents the configuration settings for the Arcanic Mediator service.
/// Contains options and settings that control the behavior of the mediator.
/// </summary>
public class ArcanicMediatorServiceConfiguration
{
    /// <summary>
    /// Gets or sets the service lifetime for the Arcanic Mediator services.
    /// Determines how instances of mediator services are created and managed by the dependency injection container.
    /// Defaults to <see cref="ServiceLifetime.Transient"/>.
    /// </summary>
    /// <value>
    /// The service lifetime. Can be <see cref="ServiceLifetime.Singleton"/>, <see cref="ServiceLifetime.Scoped"/>, 
    /// or <see cref="ServiceLifetime.Transient"/>. The default value is <see cref="ServiceLifetime.Transient"/>.
    /// </value>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
}