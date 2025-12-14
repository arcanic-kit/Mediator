using Arcanic.Mediator.Event.Abstractions;
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
    /// <summary>
    /// The service collection used for dependency injection registration.
    /// </summary>
    private readonly IServiceCollection _services;
    
    public EventModuleBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    /// Discovers and registers all event types and their corresponding handlers from the specified assembly.
    /// This method performs automatic registration by scanning for types that implement <see cref="IEvent"/>
    /// and their associated handlers that implement <see cref="IEventHandler{T}"/>, 
    /// <see cref="IEventPreHandler{TEvent}"/>, or <see cref="IEventPostHandler{TEvent}"/>.
    /// </summary>
    /// <param name="assembly">The assembly to scan for event types and handlers. All concrete classes
    /// implementing the appropriate interfaces will be registered automatically.</param>
    /// <returns>The current <see cref="EventModuleBuilder"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a handler type cannot be properly analyzed.</exception>
    public EventModuleBuilder RegisterFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var concreteTypes = assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false, IsClass: true });

        foreach (var handlerType in concreteTypes)
        {
            var eventHandlerInterfaces = handlerType.GetInterfaces()
                .Where(IsEventHandlerInterface)
                .ToList();

            if (eventHandlerInterfaces.Count == 0)
                continue;

            foreach (var eventHandlerInterface in eventHandlerInterfaces)
            {
                var eventType = eventHandlerInterface.GetGenericArguments().FirstOrDefault();
                
                if (eventType == null)
                {
                    throw new InvalidOperationException(
                        $"Unable to determine event type for handler '{handlerType.FullName}'. " +
                        $"The handler interface '{eventHandlerInterface.FullName}' does not have generic arguments.");
                }

                // Ensure the event type implements IEvent
                if (!eventType.GetInterfaces().Contains(typeof(IEvent)))
                {
                    throw new InvalidOperationException(
                        $"Event type '{eventType.FullName}' handled by '{handlerType.FullName}' must implement IEvent interface.");
                }
                
                _services.AddTransient(eventHandlerInterface, handlerType);
            }
        }

        return this;
    }

    /// <summary>
    /// Determines whether the specified type represents an event handler interface (main, pre, or post).
    /// This method checks if the type is a generic interface with the type definition <see cref="IEventHandler{T}"/>, 
    /// <see cref="IEventPreHandler{TEvent}"/>, or <see cref="IEventPostHandler{TEvent}"/>.
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
