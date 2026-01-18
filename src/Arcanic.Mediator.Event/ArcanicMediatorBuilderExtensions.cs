using System.Reflection;
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Pipeline;

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
        var eventDependencyRegistry = new EventDependencyRegistry(builder.DependencyRegistryAccessor);

        eventDependencyRegistry.RegisterEventsFromAssembly(assembly);
        eventDependencyRegistry.RegisterRequiredServices();

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
        
        ValidateEventPipelineBehaviorType(eventPipelineBehaviorType);
        
        builder.DependencyRegistryAccessor.Registry.Add(typeof(IEventPipelineBehavior<,>), eventPipelineBehaviorType);
        
        return builder;
    }
    
    /// <summary>
    /// Validates that the provided type is suitable for use as an event pipeline behavior.
    /// This method ensures the type implements the required interface, is not abstract, and is not an interface.
    /// </summary>
    /// <param name="eventPipelineBehaviorType">The type to validate for event pipeline behavior compatibility.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the type does not meet the requirements:
    /// - Must implement <see cref="IEventPipelineBehavior{TEvent, TEventResult}"/>
    /// - Cannot be abstract
    /// - Cannot be an interface
    /// </exception>
    private static void ValidateEventPipelineBehaviorType(Type eventPipelineBehaviorType)
    {
        // Check if the type implements any generic version of IEventPipelineBehavior<,>
        var implementsIEventPipelineBehavior = eventPipelineBehaviorType
            .GetInterfaces()
            .Any(interfaceType => 
                interfaceType.IsGenericType && 
                interfaceType.GetGenericTypeDefinition() == typeof(IEventPipelineBehavior<,>));

        if (!implementsIEventPipelineBehavior)
        {
            throw new ArgumentException(
                $"Type '{eventPipelineBehaviorType.FullName}' must implement '{typeof(IEventPipelineBehavior<,>).FullName}' interface.",
                nameof(eventPipelineBehaviorType));
        }
        
        // Ensure the type is not abstract and has a public constructor
        if (eventPipelineBehaviorType.IsAbstract)
        {
            throw new ArgumentException(
                $"Type '{eventPipelineBehaviorType.FullName}' cannot be abstract.",
                nameof(eventPipelineBehaviorType));
        }
        
        if (eventPipelineBehaviorType.IsInterface)
        {
            throw new ArgumentException(
                $"Type '{eventPipelineBehaviorType.FullName}' cannot be an interface.",
                nameof(eventPipelineBehaviorType));
        }
    }
}