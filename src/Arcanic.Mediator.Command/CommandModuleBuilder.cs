using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Messaging.Abstractions.Registry;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Command;

/// <summary>
/// Provides a builder for configuring command processing services by automatically discovering and registering
/// command types and their corresponding handlers from assemblies. Supports main, pre, and post command handlers.
/// </summary>
public class CommandModuleBuilder
{
    /// <summary>
    /// The service collection used for dependency injection registration.
    /// </summary>
    private readonly IServiceCollection _services;
    
    /// <summary>
    /// The message registry used for mapping command types to their handlers.
    /// </summary>
    private readonly IMessageRegistry _messageRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandModuleBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    /// <param name="messageRegistry">The message registry to register command-handler mappings with.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="messageRegistry"/> is null.</exception>
    public CommandModuleBuilder(IServiceCollection services, IMessageRegistry messageRegistry)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _messageRegistry = messageRegistry ?? throw new ArgumentNullException(nameof(messageRegistry));
    }

    /// <summary>
    /// Scans the specified assembly for command types and their corresponding handlers (main, pre, and post), 
    /// then registers them in the dependency injection container and message registry.
    /// </summary>
    /// <param name="assembly">The assembly to scan for command types and handlers. All concrete classes
    /// implementing the appropriate interfaces will be registered automatically.</param>
    /// <returns>The current <see cref="CommandModuleBuilder"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a handler type cannot be properly analyzed.</exception>
    public CommandModuleBuilder RegisterFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var concreteTypes = assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false, IsClass: true });

        foreach (var handlerType in concreteTypes)
        {
            var commandHandlerInterfaces = handlerType.GetInterfaces()
                .Where(IsCommandHandlerInterface)
                .ToList();

            if (commandHandlerInterfaces.Count == 0)
                continue;

            foreach (var commandHandlerInterface in commandHandlerInterfaces)
            {
                var commandType = commandHandlerInterface.GetGenericArguments().FirstOrDefault();
                
                if (commandType == null)
                {
                    throw new InvalidOperationException(
                        $"Unable to determine command type for handler '{handlerType.FullName}'. " +
                        $"The handler interface '{commandHandlerInterface.FullName}' does not have generic arguments.");
                }

                // Ensure the command type implements ICommand or ICommand<T>
                if (!commandType.GetInterfaces().Any(IsCommandInterface) && commandType != typeof(ICommand))
                {
                    throw new InvalidOperationException(
                        $"Command type '{commandType.FullName}' handled by '{handlerType.FullName}' must implement ICommand or ICommand<T> interface.");
                }

                _messageRegistry.Register(commandType, handlerType);
                _services.AddScoped(handlerType);
            }
        }

        return this;
    }

    /// <summary>
    /// Determines whether the specified type is a command interface.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is <see cref="ICommand"/> or <see cref="ICommand{TResult}"/>; otherwise, false.</returns>
    private static bool IsCommandInterface(Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            return genericTypeDefinition == typeof(ICommand<>);
        }   
        
        return type == typeof(ICommand);
    }

    /// <summary>
    /// Determines whether the specified type is a command handler interface (main, pre, or post).
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is a command handler interface; otherwise, false.</returns>
    private static bool IsCommandHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();
       
        return genericTypeDefinition == typeof(ICommandHandler<>) ||
               genericTypeDefinition == typeof(ICommandHandler<,>) ||
               genericTypeDefinition == typeof(ICommandPreHandler<>) ||
               genericTypeDefinition == typeof(ICommandPostHandler<>);
    }
}
