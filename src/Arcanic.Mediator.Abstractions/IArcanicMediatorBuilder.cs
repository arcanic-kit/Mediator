using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Abstractions;

/// <summary>
/// Defines the contract for building and configuring an Arcanic mediator instance.
/// Provides methods to configure pipeline behaviors and access the service collection.
/// </summary>
public interface IArcanicMediatorBuilder
{
    /// <summary>
    /// Gets the configuration settings for the Arcanic Mediator service.
    /// Contains options and settings that control the behavior of the mediator,
    /// such as service lifetime and other configuration parameters.
    /// </summary>
    /// <value>The <see cref="ArcanicMediatorServiceConfiguration"/> instance containing mediator configuration.</value>
    ArcanicMediatorServiceConfiguration Configuration { get; }

    /// <summary>
    /// Gets the service collection used for dependency injection registration.
    /// </summary>
    /// <value>The <see cref="IServiceCollection"/> instance for registering services.</value>
    IServiceCollection Services { get; }
    
    /// <summary>
    /// Adds a pipeline behavior to the mediator configuration.
    /// Pipeline behaviors allow cross-cutting concerns to be applied to request/response handling.
    /// </summary>
    /// <param name="implementationType">The type that implements the pipeline behavior interface.</param>
    /// <returns>The current builder instance to enable method chaining.</returns>
    IArcanicMediatorBuilder AddPipelineBehavior(Type implementationType);
}