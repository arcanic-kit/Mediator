namespace Arcanic.Mediator.Abstractions.DependencyInjection;

/// <summary>
/// Defines the contract for registering services with the dependency injection container.
/// This interface provides a fluent API for service registration while abstracting
/// the underlying dependency injection framework details from the mediator components.
/// </summary>
public interface IServiceRegistrar
{
    /// <summary>
    /// Registers a service type with its implementation type in the dependency injection container.
    /// The service will be registered with the instance lifetime specified in the mediator configuration.
    /// </summary>
    /// <param name="serviceType">
    /// The type of service to register. This is typically an interface or abstract class
    /// that defines the contract for the service.
    /// </param>
    /// <param name="implementationType">
    /// The concrete type that implements the service. This type must be assignable to 
    /// the <paramref name="serviceType"/> and must be a concrete, non-abstract class.
    /// </param>
    /// <returns>
    /// The current <see cref="IServiceRegistrar"/> instance to enable fluent method chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="serviceType"/> or <paramref name="implementationType"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="implementationType"/> is not assignable to <paramref name="serviceType"/>,
    /// or when <paramref name="implementationType"/> is abstract or an interface.
    /// </exception>
    IServiceRegistrar Register(Type serviceType, Type implementationType);

    /// <summary>
    /// Registers a service alias by binding an interface type to an implementation type
    /// that is already registered in the dependency injection container. This method enables
    /// multiple interface registrations for the same concrete implementation.
    /// </summary>
    /// <typeparam name="TInterface">
    /// The interface type to register as an alias. This must be a reference type (class)
    /// that is implemented by <typeparamref name="TImplementation"/>.
    /// </typeparam>
    /// <typeparam name="TImplementation">
    /// The concrete implementation type that implements <typeparamref name="TInterface"/>.
    /// This type must already be registered in the container or be registerable.
    /// </typeparam>
    /// <returns>
    /// The current <see cref="IServiceRegistrar"/> instance to enable fluent method chaining.
    /// </returns>
    IServiceRegistrar RegisterAlias<TInterface, TImplementation>()
         where TImplementation : TInterface
         where TInterface : class;
}