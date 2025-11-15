using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Messaging.Abstractions.Registry;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Event;

/// <summary>
/// Provides functionality for configuring and registering event handlers and related components
/// within the event mediator framework. This builder facilitates automatic discovery and registration
/// of event types and their corresponding handlers from assemblies.
/// </summary>
public class EventModuleBuilder
{
    private readonly IServiceCollection _services;
    private readonly IMessageRegistry _messageRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventModuleBuilder"/> class with the specified
    /// service collection and message registry.
    /// </summary>
    /// <param name="services">The dependency injection service collection used for registering discovered handlers.
    /// This parameter cannot be null.</param>
    /// <param name="messageRegistry">The message registry used for mapping event types to their handlers.
    /// This parameter cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="services"/> or 
    /// <paramref name="messageRegistry"/> is null.</exception>
    public EventModuleBuilder(IServiceCollection services, IMessageRegistry messageRegistry)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _messageRegistry = messageRegistry ?? throw new ArgumentNullException(nameof(messageRegistry));
    }

    /// <summary>
    /// Discovers and registers all event types and their corresponding handlers from the specified assembly.
    /// This method performs automatic registration by scanning for types that implement <see cref="IEvent"/>
    /// and their associated handlers that implement <see cref="IEventHandler{T}"/>.
    /// </summary>
    /// <param name="assembly">The assembly to scan for event types and handlers. All concrete classes
    /// implementing the appropriate interfaces will be registered automatically.</param>
    /// <returns>The current <see cref="EventModuleBuilder"/> instance to enable method chaining.</returns>
    /// <remarks>
    /// This method registers each discovered handler as scoped in the dependency injection container
    /// and maps them to their corresponding event types in the message registry for proper routing.
    /// </remarks>
    public EventModuleBuilder RegisterFromAssembly(Assembly assembly)
    {
        var eventTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass)
            .Where(t => t.GetInterfaces().Any(IsEventInterface))
            .ToList();

        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass)
            .Where(t => t.GetInterfaces().Any(IsEventHandlerInterface))
            .ToList();

        foreach (var eventType in eventTypes)
        {
            var matchingHandlers = handlerTypes.Where(handlerType =>
                handlerType.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IEventHandler<>)) &&
                    i.GetGenericArguments()[0] == eventType))
                .ToList();

            // Register each matching handler
            foreach (var handlerType in matchingHandlers)
            {
                _messageRegistry.Register(eventType, handlerType);
                _services.AddScoped(handlerType);
            }
        }

        return this;
    }

    /// <summary>
    /// Determines whether the specified type represents an event interface.
    /// This method checks if the type is exactly <see cref="IEvent"/> and not a generic type.
    /// </summary>
    /// <param name="type">The type to examine for event interface compatibility.</param>
    /// <returns>True if the type is the <see cref="IEvent"/> interface; otherwise, false.</returns>
    private static bool IsEventInterface(Type type)
    {
        if (type.IsGenericType)
            return false;

        return type == typeof(IEvent);
    }

    /// <summary>
    /// Determines whether the specified type represents an event handler interface.
    /// This method checks if the type is a generic interface with the type definition <see cref="IEventHandler{T}"/>.
    /// </summary>
    /// <param name="type">The type to examine for event handler interface compatibility.</param>
    /// <returns>True if the type is a generic <see cref="IEventHandler{T}"/> interface; otherwise, false.</returns>
    private static bool IsEventHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();

        return genericTypeDefinition == typeof(IEventHandler<>);
    }
}
