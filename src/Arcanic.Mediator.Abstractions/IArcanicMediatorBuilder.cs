namespace Arcanic.Mediator.Abstractions;

/// <summary>
/// Defines the contract for building and configuring an Arcanic mediator instance.
/// Provides methods to configure pipeline behaviors and access the service collection.
/// This interface follows the builder pattern to allow fluent configuration of the mediator.
/// </summary>
public interface IArcanicMediatorBuilder
{
    DependencyRegistryAccessor DependencyRegistryAccessor { get; }
    
    /// <summary>
    /// Adds a pipeline behavior to the mediator configuration.
    /// Pipeline behaviors allow cross-cutting concerns to be applied to request/response handling.
    /// Behaviors are executed in the order they are registered, forming a pipeline chain.
    /// </summary>
    /// <param name="pipelineBehaviorType">
    /// The type that implements the pipeline behavior interface.
    /// </param>
    /// <returns>The current builder instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="pipelineBehaviorType"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="pipelineBehaviorType"/> does not implement the required pipeline behavior interface,
    /// is abstract, or is an interface type.
    /// </exception>
    IArcanicMediatorBuilder AddPipelineBehavior(Type pipelineBehaviorType);
}