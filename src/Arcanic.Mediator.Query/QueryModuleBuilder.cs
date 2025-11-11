using Arcanic.Mediator.Messaging.Abstractions.Registry;
using Arcanic.Mediator.Query.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Windows.Input;

namespace Arcanic.Mediator.Query;

public class QueryModuleBuilder
{
    private readonly IServiceCollection _services;
    private readonly IMessageRegistry _messageRegistry;

    public QueryModuleBuilder(IServiceCollection services, IMessageRegistry messageRegistry)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _messageRegistry = messageRegistry ?? throw new ArgumentNullException(nameof(messageRegistry));
    }

    public QueryModuleBuilder RegisterFromAssembly(Assembly assembly)
    {
        var queryTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass)
            .Where(t => t.GetInterfaces().Any(IsQueryInterface))
            .ToList();

        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass)
            .Where(t => t.GetInterfaces().Any(IsQueryHandlerInterface))
            .ToList();

        foreach (var queryType in queryTypes)
        {
            var matchingHandlers = handlerTypes.Where(handlerType =>
                handlerType.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)) &&
                    i.GetGenericArguments()[0] == queryType))
                .ToList();

            // Register each matching handler
            foreach (var handlerType in matchingHandlers)
            {
                _messageRegistry.Register(queryType, handlerType);
                _services.AddScoped(handlerType);
            }
        }

        return this;
    }

    private static bool IsQueryInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();
        return genericTypeDefinition == typeof(IQuery<>);
    }

    private static bool IsQueryHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();

        return genericTypeDefinition == typeof(IQueryHandler<,>);
    }
}
