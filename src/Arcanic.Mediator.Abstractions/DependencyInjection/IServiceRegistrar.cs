using Arcanic.Mediator.Abstractions.Configuration;

namespace Arcanic.Mediator.Abstractions.DependencyInjection;

/// <summary>
/// Defines the contract for registering services with the dependency injection container.
/// This interface provides a fluent API for service registration while abstracting
/// the underlying dependency injection framework details from the mediator components.
/// </summary>
public interface IServiceRegistrar
{
    /// <summary>
    /// Registers a service type with its implementation type using the configured handler lifetime
    /// (<see cref="ArcanicMediatorConfiguration.HandlerLifetime"/>).
    /// </summary>
    /// <param name="serviceType">The type of service to register.</param>
    /// <param name="implementationType">The implementation type that provides the service.</param>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="serviceType"/> or <paramref name="implementationType"/> is <c>null</c>.
    /// </exception>
    IServiceRegistrar Register(Type serviceType, Type implementationType);

    /// <summary>
    /// Registers a service type with its implementation type using the configured instance lifetime,
    /// unless that exact service/implementation pair is already registered.
    /// Use this for registrations that must stay idempotent when configuration methods are called more than once.
    /// </summary>
    /// <param name="serviceType">The type of service to register.</param>
    /// <param name="implementationType">The implementation type that provides the service.</param>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="serviceType"/> or <paramref name="implementationType"/> is <c>null</c>.
    /// </exception>
    IServiceRegistrar TryRegister(Type serviceType, Type implementationType);

    /// <summary>
    /// Registers a service type with its implementation type using an explicit lifetime,
    /// unless that exact service/implementation pair is already registered.
    /// Used for mediator entry points, whose lifetime is <see cref="ArcanicMediatorConfiguration.MediatorLifetime"/>.
    /// </summary>
    /// <param name="serviceType">The type of service to register.</param>
    /// <param name="implementationType">The implementation type that provides the service.</param>
    /// <param name="lifetime">The lifetime to register the service with.</param>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="serviceType"/> or <paramref name="implementationType"/> is <c>null</c>.
    /// </exception>
    IServiceRegistrar TryRegister(Type serviceType, Type implementationType, InstanceLifetime lifetime);

    /// <summary>
    /// Gets the mediator configuration this registrar applies.
    /// </summary>
    ArcanicMediatorConfiguration Configuration { get; }

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