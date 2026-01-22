using System.Reflection;
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Pipeline;
using Arcanic.Mediator.Event.DependencyInjection;

namespace Arcanic.Mediator.Event;

/// <summary>
/// Provides extension methods for <see cref="IArcanicMediatorBuilder"/> to register event handlers and related services.
/// </summary>
public static class ArcanicMediatorBuilderExtensions
{
    /// <summary>
    /// Registers all event handlers found in the specified assembly along with required event services.
    /// This method scans the assembly for event implementations and registers them with the dependency injection container.
    /// </summary>
    /// <param name="builder">The mediator builder instance to extend.</param>
    /// <param name="assembly">The assembly to scan for event handlers.</param>
    /// <returns>The mediator builder instance to enable method chaining.</returns>
    public static IArcanicMediatorBuilder AddEvents(this IArcanicMediatorBuilder builder, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(assembly);

        builder.ServiceRegistrar.RegisterEventsFromAssembly(assembly);
        builder.ServiceRegistrar.RegisterEventRequiredServices();

        return builder;
    }
    
    /// <summary>
    /// Registers a custom pipeline behavior for event processing in the mediator pipeline.
    /// Pipeline behaviors allow cross-cutting concerns such as logging, validation, caching, 
    /// or transaction management to be applied to event handling.
    /// </summary>
    /// <param name="builder">The mediator builder instance to extend.</param>
    /// <param name="eventPipelineBehaviorType">
    /// The type of the pipeline behavior to register. Must implement <see cref="IEventPipelineBehavior{TEvent, TEventResult}"/>.
    /// </param>
    /// <returns>The mediator builder instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="builder"/> or <paramref name="eventPipelineBehaviorType"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="eventPipelineBehaviorType"/> does not implement the required interface,
    /// is abstract, or is an interface type.
    /// </exception>
    public static IArcanicMediatorBuilder AddEventPipelineBehavior(this IArcanicMediatorBuilder builder,
        Type eventPipelineBehaviorType)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(eventPipelineBehaviorType);

        builder.ServiceRegistrar.RegisterEventPipelineBehavior(eventPipelineBehaviorType);

        return builder;
    }
}