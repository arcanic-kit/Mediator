using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Messaging.Abstractions.Registry;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Command;

public class CommandModuleBuilder
{
    private readonly IServiceCollection _services;
    private readonly IMessageRegistry _messageRegistry;

    public CommandModuleBuilder(IServiceCollection services, IMessageRegistry messageRegistry)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _messageRegistry = messageRegistry ?? throw new ArgumentNullException(nameof(messageRegistry));
    }

    public CommandModuleBuilder RegisterFromAssembly(Assembly assembly)
    {
        var commandTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass)
            .Where(t => t.GetInterfaces().Any(IsCommandInterface))
            .ToList();

        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass)
            .Where(t => t.GetInterfaces().Any(IsCommandHandlerInterface))
            .ToList();

        foreach (var commandType in commandTypes)
        {
            var matchingHandlers = handlerTypes.Where(handlerType => 
                handlerType.GetInterfaces().Any(i => 
                    i.IsGenericType && 
                    (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) || i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)) &&
                    i.GetGenericArguments()[0] == commandType))
                .ToList();

            // Register each matching handler
            foreach (var handlerType in matchingHandlers)
            {
                _messageRegistry.Register(commandType, handlerType);
                _services.AddScoped(handlerType);
            }
        }

        return this;
    }

    private static bool IsCommandInterface(Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            return genericTypeDefinition == typeof(ICommand<>);
        }   
        
        return type == typeof(ICommand);
    }

    private static bool IsCommandHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();
       
        return genericTypeDefinition == typeof(ICommandHandler<>) ||
               genericTypeDefinition == typeof(ICommandHandler<,>);
    }
}
