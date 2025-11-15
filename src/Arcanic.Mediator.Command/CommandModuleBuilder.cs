using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Messaging.Abstractions.Registry;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Command;

/// <summary>
/// Provides a builder for configuring command processing services by automatically discovering and registering
/// command types and their corresponding handlers from assemblies.
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
    /// Scans the specified assembly for command types and their corresponding handlers, then registers them
    /// in the dependency injection container and message registry.
    /// </summary>
    /// <param name="assembly">The assembly to scan for command types and handlers.</param>
    /// <returns>The current <see cref="CommandModuleBuilder"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
    /// <remarks>
    /// This method automatically discovers:
    /// - Classes implementing <see cref="ICommand"/> or <see cref="ICommand{TResult}"/>
    /// - Classes implementing <see cref="ICommandHandler{TCommand}"/> or <see cref="ICommandHandler{TCommand, TResult}"/>
    /// It then registers matching command-handler pairs in both the DI container and message registry.
    /// </remarks>
    public CommandModuleBuilder RegisterFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

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
    /// Determines whether the specified type is a command handler interface.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is <see cref="ICommandHandler{TCommand}"/> or <see cref="ICommandHandler{TCommand, TResult}"/>; otherwise, false.</returns>
    private static bool IsCommandHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();
       
        return genericTypeDefinition == typeof(ICommandHandler<>) ||
               genericTypeDefinition == typeof(ICommandHandler<,>);
    }
}
