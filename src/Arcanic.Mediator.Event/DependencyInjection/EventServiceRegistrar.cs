using Arcanic.Mediator.Abstractions.DependencyInjection;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Handler;
using Arcanic.Mediator.Event.Abstractions.Pipeline;
using Arcanic.Mediator.Event.Pipeline;
using System.Reflection;

namespace Arcanic.Mediator.Event.DependencyInjection;

/// <summary>
/// Provides functionality for registering event-related services and handlers with the dependency injection container.
/// This class handles automatic discovery and registration of event handlers, pre-handlers, post-handlers,
/// and required pipeline services from assemblies.
/// </summary>
public class EventServiceRegistrar
{
    /// <summary>
    /// The service registrar used to register services in the dependency injection container.
    /// </summary>
    private readonly IServiceRegistrar _serviceRegistrar;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="EventServiceRegistrar"/> class.
    /// </summary>
    /// <param name="serviceRegistrar">The service registrar used to register event-related services and handlers.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceRegistrar"/> is <c>null</c>.</exception>
    public EventServiceRegistrar(IServiceRegistrar serviceRegistrar)
    {
        _serviceRegistrar = serviceRegistrar ?? throw new ArgumentNullException(nameof(serviceRegistrar));
    }
    
    /// <summary>
    /// Registers the core services required for event processing, including the event publisher
    /// and default pipeline behaviors for pre and post-processing.
    /// </summary>
    /// <returns>The current <see cref="EventServiceRegistrar"/> instance to enable method chaining.</returns>
    public EventServiceRegistrar RegisterRequiredServices()
    {
        _serviceRegistrar
            .Register(typeof(IEventPublisher), typeof(EventPublisher))
            .Register(typeof(IEventPipelineBehavior<,>), typeof(EventPostHandlerPipelineBehavior<,>))
            .Register(typeof(IEventPipelineBehavior<,>), typeof(EventPreHandlerPipelineBehavior<,>));
        
        return this;
    }
    
    /// <summary>
    /// Discovers and registers all event types and their corresponding handlers from the specified assembly.
    /// This method performs automatic registration by scanning for types that implement <see cref="IEvent"/>
    /// and their associated handlers that implement <see cref="IEventHandler{TEvent}"/>, <see cref="IEventPreHandler{TEvent}"/>
    /// , or <see cref="IEventPostHandler{TEvent}"/>.
    /// </summary>
    /// <param name="assembly">The assembly to scan for event types and handlers. All concrete classes
    /// implementing the appropriate interfaces will be registered automatically.</param>
    /// <returns>The current <see cref="EventServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a handler type cannot be properly analyzed, typically when:
    /// <list type="bullet">
    /// <item><description>A handler interface does not have the expected generic arguments</description></item>
    /// <item><description>An event type does not implement the required <see cref="IEvent"/> interface</description></item>
    /// </list>
    /// </exception>
    public EventServiceRegistrar RegisterEventsFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var eventHandlerRegistrations = assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false, IsClass: true })
            .SelectMany(handlerType => handlerType.GetInterfaces()
                .Where(IsEventHandlerInterface)
                .Select(eventHandlerInterface => new { handlerType, eventHandlerInterface }))
            .Where(registration =>
            {
                var eventType = registration.eventHandlerInterface.GetGenericArguments().FirstOrDefault();

                if (eventType == null)
                {
                    throw new InvalidOperationException(
                        $"Unable to determine Event type for handler '{registration.handlerType.FullName}'. " +
                        $"The handler interface '{registration.eventHandlerInterface.FullName}' does not have generic arguments.");
                }

                if (!eventType.GetInterfaces().Any(IsEventInterface))
                {
                    throw new InvalidOperationException(
                        $"Event type '{eventType.FullName}' handled by '{registration.handlerType.FullName}' must implement Event interface.");
                }

                return true;
            });
        
        foreach (var registration in eventHandlerRegistrations)
        {
            _serviceRegistrar.Register(registration.eventHandlerInterface, registration.handlerType);
        }

        return this;
    }

	/// <summary>
	/// Registers a custom event pipeline behavior with the dependency injection container.
	/// The pipeline behavior will be executed as part of the event processing pipeline,
	/// allowing for cross-cutting concerns such as logging, validation, auditing, or performance monitoring.
	/// </summary>
	/// <param name="eventPipelineBehaviorType">
	/// The type that implements <see cref="IEventPipelineBehavior{TEvent, TEventResult}"/>.
	/// Must be a concrete class that is not abstract or an interface.
	/// </param>
	/// <returns>The current <see cref="EventServiceRegistrar"/> instance to enable method chaining.</returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="eventPipelineBehaviorType"/> is <c>null</c>.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Thrown when <paramref name="eventPipelineBehaviorType"/> does not implement the required 
	/// <see cref="IEventPipelineBehavior{TEvent, TEventResult}"/> interface, is abstract, or is an interface type.
	/// </exception>
	public EventServiceRegistrar RegisterEventPipelineBehavior(Type eventPipelineBehaviorType)
	{
		ArgumentNullException.ThrowIfNull(eventPipelineBehaviorType);

		ValidateEventPipelineBehaviorType(eventPipelineBehaviorType);

        _serviceRegistrar.Register(typeof(IEventPipelineBehavior<,>), eventPipelineBehaviorType);

		return this;
	}

	/// <summary>
	/// Determines whether the specified type represents the <see cref="IEvent"/> interface.
	/// This method handles both generic and non-generic forms of the IEvent interface.
	/// </summary>
	/// <param name="type">The type to examine for event interface compatibility.</param>
	/// <returns>True if the type is <see cref="IEvent"/> or a generic form of IEvent; otherwise, false.</returns>
	private static bool IsEventInterface(Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            return genericTypeDefinition == typeof(IEvent);
        }

        return type == typeof(IEvent);
    }

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
