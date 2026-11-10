using Arcanic.Mediator.Abstractions.DependencyInjection;
using Arcanic.Mediator.DependencyInjection;
using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Request.Abstractions.Pipeline;

namespace Arcanic.Mediator.Request.DependencyInjection;

/// <summary>
/// Provides extension methods for <see cref="IServiceRegistrar"/> to register request pipeline behaviors
/// and other mediator-related services for dependency injection.
/// </summary>
public static class ServiceRegistrarExtensions
{
    /// <summary>
    /// Registers the request mediator service with the dependency injection container.
    /// This registers the <see cref="IMediator"/> interface with its default implementation.
    /// </summary>
    /// <param name="serviceRegistrar">The service registrar to register the pipeline behavior with.</param>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="serviceRegistrar"/> is <c>null</c>.
    /// </exception>
    public static IServiceRegistrar RegisterRequestRequiredServices(this IServiceRegistrar serviceRegistrar)
    {
        ArgumentNullException.ThrowIfNull(serviceRegistrar);

        if (!serviceRegistrar.IsRegistered(typeof(IMediator)))
            serviceRegistrar.TryRegister(typeof(IMediator), typeof(Mediator));

        return serviceRegistrar;
    }

    /// <summary>
    /// Registers a custom request pipeline behavior with the dependency injection container.
    /// The pipeline behavior will be executed as part of the request processing pipeline,
    /// allowing for cross-cutting concerns such as logging, validation, caching, or performance monitoring.
    /// </summary>
    /// <param name="serviceRegistrar">The service registrar to register the pipeline behavior with.</param>
    /// <param name="requestPipelineBehaviorType">
    /// The type that implements <see cref="IRequestPipelineBehavior{TRequest, TResponse}"/>.
    /// Must be a concrete class that is not abstract or an interface.
    /// </param>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="requestPipelineBehaviorType"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="requestPipelineBehaviorType"/> does not implement the required 
    /// <see cref="IRequestPipelineBehavior{TRequest, TResponse}"/> interface, is abstract, or is an interface type.
    /// </exception>
    public static IServiceRegistrar RegisterRequestPipelineBehavior(this IServiceRegistrar serviceRegistrar, Type requestPipelineBehaviorType)
    {
        PipelineBehaviorValidator.Validate(requestPipelineBehaviorType, typeof(IRequestPipelineBehavior<,>), nameof(requestPipelineBehaviorType));

        serviceRegistrar.TryRegister(typeof(IRequestPipelineBehavior<,>), requestPipelineBehaviorType);

        return serviceRegistrar;
    }


}
