using Arcanic.Mediator.Command.Abstractions;
using System.Reflection;
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Command.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Pipeline;

namespace Arcanic.Mediator.Command;

/// <summary>
/// Provides functionality for registering command-related services and handlers with the dependency injection container.
/// This class handles automatic discovery and registration of command handlers, pre-handlers, post-handlers,
/// and required pipeline services from assemblies.
/// </summary>
public class CommandDependencyRegistry
{
    /// <summary>
    /// Lazy singleton accessor for the DependencyRegistry instance that manages service registrations.
    /// </summary>
    private readonly DependencyRegistryAccessor _dependencyRegistryAccessor;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandDependencyRegistry"/> class.
    /// </summary>
    /// <param name="dependencyRegistryAccessor">The accessor for the dependency registry where services will be registered.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependencyRegistryAccessor"/> is null.</exception>
    public CommandDependencyRegistry(DependencyRegistryAccessor dependencyRegistryAccessor)
    {
        _dependencyRegistryAccessor = dependencyRegistryAccessor;
    }

    /// <summary>
    /// Registers the core services required for command processing, including the command mediator
    /// and default pipeline behaviors for pre and post-processing.
    /// </summary>
    /// <returns>The current <see cref="CommandDependencyRegistry"/> instance to enable method chaining.</returns>
    public CommandDependencyRegistry RegisterRequiredServices()
    {
        _dependencyRegistryAccessor.Registry
            .Add(typeof(ICommandMediator), typeof(CommandMediator))
            .Add(typeof(ICommandPipelineBehavior<,>), typeof(CommandPostHandlerPipelineBehavior<,>))
            .Add(typeof(ICommandPipelineBehavior<,>), typeof(CommandPreHandlerPipelineBehavior<,>));

        return this;
    }

    /// <summary>
    /// Scans the specified assembly for command types and their corresponding handlers (main, pre, and post),
    /// then registers them in the dependency injection container and message registry.
    /// </summary>
    /// <param name="assembly">The assembly to scan for command types and handlers. All concrete classes
    /// implementing the appropriate interfaces will be registered automatically.</param>
    /// <returns>The current <see cref="CommandDependencyRegistry"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a handler type cannot be properly analyzed.</exception>
    public CommandDependencyRegistry RegisterCommandsFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var commandHandlerRegistrations = assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false, IsClass: true })
            .SelectMany(handlerType => handlerType.GetInterfaces()
                .Where(IsCommandHandlerInterface)
                .Select(commandHandlerInterface => new { handlerType, commandHandlerInterface }))
            .Where(registration =>
            {
                var commandType = registration.commandHandlerInterface.GetGenericArguments().FirstOrDefault();

                if (commandType == null)
                {
                    throw new InvalidOperationException(
                        $"Unable to determine command type for handler '{registration.handlerType.FullName}'. " +
                        $"The handler interface '{registration.commandHandlerInterface.FullName}' does not have generic arguments.");
                }

                if (!commandType.GetInterfaces().Any(IsCommandInterface))
                {
                    throw new InvalidOperationException(
                        $"Command type '{commandType.FullName}' handled by '{registration.handlerType.FullName}' must implement ICommand<T> interface.");
                }

                return true;
            });
        
        foreach (var registration in commandHandlerRegistrations)
        {
            _dependencyRegistryAccessor.Registry.Add(registration.commandHandlerInterface, registration.handlerType);
        }

        return this;
    }

    /// <summary>
    /// Determines whether the specified type represents a command interface.
    /// This method handles both generic and non-generic forms of command interfaces.
    /// </summary>
    /// <param name="type">The type to check for command interface implementation.</param>
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
    /// Determines whether the specified type represents a command handler interface (main, pre, or post).
    /// This method checks if the type is a generic interface matching any of the supported command handler patterns:
    /// <see cref="ICommandHandler{TCommand}"/>, <see cref="ICommandHandler{TCommand, TResult}"/>, 
    /// <see cref="ICommandPreHandler{TCommand}"/>, or <see cref="ICommandPostHandler{TCommand}"/>.
    /// </summary>
    /// <param name="type">The type to examine for command handler interface compatibility.</param>
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
