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

    T GetRequiredService<T>() where T : class;
}