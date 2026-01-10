using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator;

/// <summary>
/// Default implementation of the Arcanic mediator builder for configuring mediator services.
/// </summary>
public class DefaultArcanicMediatorBuilder: IArcanicMediatorBuilder
{
    /// <summary>
    /// Gets the configuration settings for the Arcanic mediator service.
    /// </summary>
    public ArcanicMediatorServiceConfiguration Configuration { get; }
    
    /// <summary>
    /// Gets the service collection used for dependency injection registration.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Initializes a new instance of the DefaultArcanicMediatorBuilder class.
    /// </summary>
    /// <param name="services">The service collection for dependency injection.</param>
    /// <param name="configuration">The configuration settings for the mediator service.</param>
    public DefaultArcanicMediatorBuilder(IServiceCollection services, ArcanicMediatorServiceConfiguration configuration)
    {
        Services = services;
        Configuration = configuration;
    }

    /// <summary>
    /// Adds a pipeline behavior to the mediator configuration.
    /// </summary>
    /// <param name="implementationType">The type implementing the pipeline behavior.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public IArcanicMediatorBuilder AddPipelineBehavior(Type implementationType)
    {
        Services.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), implementationType, Configuration.Lifetime));

        return this;
    }
}