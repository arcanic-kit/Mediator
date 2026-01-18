namespace Arcanic.Mediator.Abstractions.Configuration;

/// <summary>
/// Specifies the lifetime of a service in the dependency injection container.
/// </summary>
public enum InstanceLifetime
{
    /// <summary>
    /// The service is created each time it is requested. This is the default behavior.
    /// </summary>
    Transient,

    /// <summary>
    /// The service is created once per request (connection). It is shared within the request but not across requests.
    /// </summary>
    Scoped,

    /// <summary>
    /// The service is created once and shared throughout the application's lifetime.
    /// </summary>
    Singleton
}