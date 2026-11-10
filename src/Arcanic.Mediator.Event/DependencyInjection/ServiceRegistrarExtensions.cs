using Arcanic.Mediator.Abstractions.DependencyInjection;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Event.Abstractions.Pipeline;
using Arcanic.Mediator.Event.Pipeline;
using Arcanic.Mediator.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Event.DependencyInjection;

/// <summary>
/// Provides extension methods for <see cref="IServiceRegistrar"/> to support registration of event-related services.
/// This class contains methods for registering event handlers, pipeline behaviors, and core event processing infrastructure
/// components required by the Arcanic Mediator event system.
/// </summary>
public static class ServiceRegistrarExtensions
{
    /// <summary>
    /// Registers the core services required for event processing, including the event publisher
    /// and default pipeline behaviors for pre and post-processing.
    /// </summary>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    public static IServiceRegistrar RegisterEventRequiredServices(this IServiceRegistrar serviceRegistrar)
    {
        serviceRegistrar
            .TryRegister(typeof(IPublisher), typeof(Publisher))
            .TryRegister(typeof(IEventPipelineBehavior<,>), typeof(EventPostHandlerPipelineBehavior<,>))
            .TryRegister(typeof(IEventPipelineBehavior<,>), typeof(EventPreHandlerPipelineBehavior<,>));

        return serviceRegistrar;
    }

    /// <summary>
    /// Discovers and registers all event types and their corresponding handlers from the specified assembly.
    /// This method performs automatic registration by scanning for types that implement <see cref="IEvent"/>
    /// and their associated handlers that implement <see cref="IEventHandler{TEvent}"/>, <see cref="IEventPreHandler{TEvent}"/>
    /// , or <see cref="IEventPostHandler{TEvent}"/>.
    /// </summary>
    /// <param name="serviceRegistrar">The service registrar to register the pipeline behavior with.</param>
    /// <param name="assembly">The assembly to scan for event types and handlers. All concrete classes
    /// implementing the appropriate interfaces will be registered automatically.</param>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a handler type cannot be properly analyzed, typically when:
    /// <list type="bullet">
    /// <item><description>A handler interface does not have the expected generic arguments</description></item>
    /// <item><description>An event type does not implement the required <see cref="IEvent"/> interface</description></item>
    /// </list>
    /// </exception>
    public static IServiceRegistrar RegisterEventsFromAssembly(this IServiceRegistrar serviceRegistrar, Assembly assembly) =>
        HandlerAssemblyScanner.RegisterHandlers(serviceRegistrar, assembly, IsEventHandlerInterface, IsEventInterface, "IEvent");

    /// <summary>
    /// Registers a custom event pipeline behavior with the dependency injection container.
    /// The pipeline behavior will be executed as part of the event processing pipeline,
    /// allowing for cross-cutting concerns such as logging, validation, auditing, or performance monitoring.
    /// </summary>
    /// <param name="serviceRegistrar">The service registrar to register the pipeline behavior with.</param>
    /// <param name="eventPipelineBehaviorType">
    /// The type that implements <see cref="IEventPipelineBehavior{TEvent, TEventResult}"/>.
    /// Must be a concrete class that is not abstract or an interface.
    /// </param>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="eventPipelineBehaviorType"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="eventPipelineBehaviorType"/> does not implement the required 
    /// <see cref="IEventPipelineBehavior{TEvent, TEventResult}"/> interface, is abstract, or is an interface type.
    /// </exception>
    public static IServiceRegistrar RegisterEventPipelineBehavior(this IServiceRegistrar serviceRegistrar, Type eventPipelineBehaviorType)
    {
        PipelineBehaviorValidator.Validate(eventPipelineBehaviorType, typeof(IEventPipelineBehavior<,>), nameof(eventPipelineBehaviorType));

        serviceRegistrar.TryRegister(typeof(IEventPipelineBehavior<,>), eventPipelineBehaviorType);

        return serviceRegistrar;
    }

    /// <summary>
    /// Determines whether the specified type represents the <see cref="IEvent"/> interface.
    /// </summary>
    /// <param name="type">The type to examine for event interface compatibility.</param>
    /// <returns>True if the type is <see cref="IEvent"/>; otherwise, false.</returns>
    private static bool IsEventInterface(Type type) => type == typeof(IEvent);

    /// <summary>
    /// Determines whether the specified type represents an event handler interface (main, pre, or post).
    /// This method checks if the type is a generic interface matching any of the supported event handler patterns:
    /// <see cref="IEventHandler{TEvent}"/>, <see cref="IEventPreHandler{TEvent}"/>, or <see cref="IEventPostHandler{TEvent}"/>.
    /// </summary>
    /// <param name="type">The type to examine for event handler interface compatibility.</param>
    /// <returns>True if the type is an event handler interface; otherwise, false.</returns>
    private static bool IsEventHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();

        return genericTypeDefinition == typeof(IEventHandler<>) ||
               genericTypeDefinition == typeof(IEventPreHandler<>) ||
               genericTypeDefinition == typeof(IEventPostHandler<>);
    }
}
