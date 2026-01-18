namespace Arcanic.Mediator.Abstractions.Configuration;

/// <summary>
/// Represents the configuration settings for the Arcanic Mediator service.
/// Contains options and settings that control the behavior of the mediator.
/// </summary>
public class ArcanicMediatorServiceConfiguration
{
    /// <summary>
    /// Gets or sets the service lifetime for the Arcanic Mediator services.
    /// Determines how instances of mediator services are created and managed by the dependency injection container.
    /// Defaults to <see cref="InstanceLifetime.Transient"/>.
    /// </summary>
    /// <value>
    /// The service lifetime. Can be <see cref="InstanceLifetime.Singleton"/>, <see cref="InstanceLifetime.Scoped"/>, 
    /// or <see cref="InstanceLifetime.Transient"/>. The default value is <see cref="InstanceLifetime.Transient"/>.
    /// </value>
    public InstanceLifetime InstanceLifetime { get; set; } = InstanceLifetime.Transient;
}