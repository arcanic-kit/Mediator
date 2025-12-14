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

    /// <summary>
    /// Initializes a new instance of the <see cref="EventModuleBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection to use for dependency injection registration.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is null.</exception>
    public EventModuleBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    /// Registers all event handler types from the specified assembly.
    /// Discovers types implementing <see cref="IEventHandler{T}"/>, <see cref="IEventPreHandler{TEvent}"/>, or <see cref="IEventPostHandler{TEvent}"/>
    /// and registers them as transient services in the dependency injection container.
    /// </summary>
    /// <param name="assembly">The assembly to scan for event handler types.</param>
    /// <returns>The current <see cref="EventModuleBuilder"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a handler interface does not have generic arguments or if the event type does not implement <see cref="IEvent"/>.
    /// </exception>
    public EventModuleBuilder RegisterFromAssembly(Assembly assembly)
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
            _services.AddTransient(registration.eventHandlerInterface, registration.handlerType);
        }

        return this;
    }

    /// <summary>
    /// Determines whether the specified type represents the <see cref="IEvent"/> interface.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is <see cref="IEvent"/>; otherwise, false.</returns>
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
