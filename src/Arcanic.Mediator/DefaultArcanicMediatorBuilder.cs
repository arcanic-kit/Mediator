using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Configuration;
using Arcanic.Mediator.Abstractions.DependencyInjection;
using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator;

/// <summary>
/// Default implementation of the Arcanic mediator builder for configuring mediator services.
/// </summary>
public class DefaultArcanicMediatorBuilder: IArcanicMediatorBuilder
{
    /// <summary>
    /// Lazy singleton accessor for the DependencyRegistry instance.
    /// </summary>
    public IServiceRegistrar ServiceRegistrar { get; }
    
    /// <summary>
    /// Initializes a new instance of the DefaultArcanicMediatorBuilder class.
    /// </summary>
    /// <param name="services">The service collection for dependency injection.</param>
    /// <param name="configuration">The configuration settings for the mediator service.</param>
    public DefaultArcanicMediatorBuilder(IServiceCollection services, ArcanicMediatorConfiguration configuration)
    {
        ServiceRegistrar = new ServiceRegistrar(configuration, services);
    }

    /// <summary>
    /// Adds a pipeline behavior to the mediator configuration.
    /// </summary>
    /// <param name="pipelineBehaviorType">The type implementing the pipeline behavior.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when pipelineBehaviorType is null.</exception>
    /// <exception cref="ArgumentException">Thrown when pipelineBehaviorType does not implement IPipelineBehavior interface.</exception>
    public IArcanicMediatorBuilder AddPipelineBehavior(Type pipelineBehaviorType)
    {
        ArgumentNullException.ThrowIfNull(pipelineBehaviorType);
        
        ValidatePipelineBehaviorType(pipelineBehaviorType);

        ServiceRegistrar.Register(typeof(IPipelineBehavior<,>), pipelineBehaviorType);

        return this;
    }
    
    /// <summary>
    /// Validates that the specified type implements the IPipelineBehavior interface.
    /// </summary>
    /// <param name="pipelineBehaviorType">The type to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the type does not implement IPipelineBehavior interface.</exception>
    private static void ValidatePipelineBehaviorType(Type pipelineBehaviorType)
    {
        // Check if the type implements any generic version of IPipelineBehavior<,>
        var implementsIPipelineBehavior = pipelineBehaviorType
            .GetInterfaces()
            .Any(interfaceType => 
                interfaceType.IsGenericType && 
                interfaceType.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

        if (!implementsIPipelineBehavior)
        {
            throw new ArgumentException(
                $"Type '{pipelineBehaviorType.FullName}' must implement '{typeof(IPipelineBehavior<,>).FullName}' interface.",
                nameof(pipelineBehaviorType));
        }
        
        // Ensure the type is not abstract and has a public constructor
        if (pipelineBehaviorType.IsAbstract)
        {
            throw new ArgumentException(
                $"Type '{pipelineBehaviorType.FullName}' cannot be abstract.",
                nameof(pipelineBehaviorType));
        }
        
        if (pipelineBehaviorType.IsInterface)
        {
            throw new ArgumentException(
                $"Type '{pipelineBehaviorType.FullName}' cannot be an interface.",
                nameof(pipelineBehaviorType));
        }
    }
}