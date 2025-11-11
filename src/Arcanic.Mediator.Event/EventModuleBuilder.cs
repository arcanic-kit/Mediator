using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Messaging.Abstractions.Registry;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Event;

public class EventModuleBuilder
{
    private readonly IServiceCollection _services;
    private readonly IMessageRegistry _messageRegistry;

    public EventModuleBuilder(IServiceCollection services, IMessageRegistry messageRegistry)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _messageRegistry = messageRegistry ?? throw new ArgumentNullException(nameof(messageRegistry));
    }

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

    private static bool IsEventInterface(Type type)
    {
        if (type.IsGenericType)
            return false;

        return type == typeof(IEvent);
    }

    private static bool IsEventHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();

        return genericTypeDefinition == typeof(IEventHandler<>);
    }
}
