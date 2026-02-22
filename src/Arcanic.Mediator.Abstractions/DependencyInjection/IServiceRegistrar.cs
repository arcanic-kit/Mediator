namespace Arcanic.Mediator.Abstractions.DependencyInjection;

/// <summary>
/// Defines the contract for registering services with the dependency injection container.
/// This interface provides a fluent API for service registration while abstracting
/// the underlying dependency injection framework details from the mediator components.
/// </summary>
public interface IServiceRegistrar
{
    /// <summary>
    /// Registers a service type with its implementation type using the configured instance lifetime.
    /// </summary>
    /// <param name="serviceType">The type of service to register.</param>
    /// <param name="implementationType">The implementation type that provides the service.</param>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="serviceType"/> or <paramref name="implementationType"/> is <c>null</c>.
    /// </exception>
    IServiceRegistrar Register(Type serviceType, Type implementationType);

    /// <summary>
    /// Validates whether a service type is registered in the service collection.
    /// </summary>
    /// <param name="serviceType">The type of service to check for registration.</param>
    /// <returns><c>true</c> if the service type is registered; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="serviceType"/> is <c>null</c>.
    /// </exception>
    bool IsRegistered(Type serviceType);
}