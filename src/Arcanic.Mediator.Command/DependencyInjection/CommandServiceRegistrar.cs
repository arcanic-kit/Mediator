using Arcanic.Mediator.Abstractions.DependencyInjection;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Command.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Pipeline;
using System.Reflection;

namespace Arcanic.Mediator.Command.DependencyInjection;

/// <summary>
/// Provides functionality for registering command-related services and handlers with the dependency injection container.
/// This class handles automatic discovery and registration of command handlers, pre-handlers, post-handlers,
/// and required pipeline services from assemblies.
/// </summary>
public class CommandServiceRegistrar
{
    private readonly IServiceRegistrar _serviceRegistrar;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandServiceRegistrar"/> class.
    /// </summary>
    /// <param name="serviceRegistrar">The service registrar used to register command-related services and handlers.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceRegistrar"/> is <c>null</c>.</exception>
    public CommandServiceRegistrar(IServiceRegistrar serviceRegistrar)
    {
        _serviceRegistrar = serviceRegistrar ?? throw new ArgumentNullException(nameof(serviceRegistrar));
    }

    /// <summary>
    /// Registers the core services required for command processing, including the command mediator
    /// and default pipeline behaviors for pre and post-processing.
    /// </summary>
    /// <returns>The current <see cref="CommandServiceRegistrar"/> instance to enable method chaining.</returns>
    public CommandServiceRegistrar RegisterRequiredServices()
    {
        _serviceRegistrar
            .Register(typeof(ICommandMediator), typeof(CommandMediator))
            .Register(typeof(ICommandPipelineBehavior<,>), typeof(CommandPostHandlerPipelineBehavior<,>))
            .Register(typeof(ICommandPipelineBehavior<,>), typeof(CommandPreHandlerPipelineBehavior<,>));

        return this;
    }

    /// <summary>
    /// Scans the specified assembly for command types and their corresponding handlers (main, pre, and post),
    /// then registers them in the dependency injection container and message registry.
    /// </summary>
    /// <param name="assembly">The assembly to scan for command types and handlers. All concrete classes
    /// implementing the appropriate interfaces will be registered automatically.</param>
    /// <returns>The current <see cref="CommandServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a handler type cannot be properly analyzed.</exception>
    public CommandServiceRegistrar RegisterCommandsFromAssembly(Assembly assembly)
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
            _serviceRegistrar.Register(registration.commandHandlerInterface, registration.handlerType);
        }

        return this;
    }

    /// <summary>
    /// Registers a command pipeline behavior with the dependency injection container.
    /// The pipeline behavior will be executed as part of the command processing pipeline,
    /// allowing for cross-cutting concerns such as logging, validation, or caching.
    /// </summary>
    /// <param name="commandPipelineBehaviorType">
    /// The type that implements <see cref="ICommandPipelineBehavior{TCommand, TResult}"/>.
    /// Must be a concrete class that is not abstract or an interface.
    /// </param>
    /// <returns>The current <see cref="CommandServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="commandPipelineBehaviorType"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="commandPipelineBehaviorType"/> does not implement the required 
    /// <see cref="ICommandPipelineBehavior{TCommand, TResult}"/> interface, is abstract, or is an interface type.
    /// </exception>
    public CommandServiceRegistrar RegisterCommandPipelineBehavior(Type commandPipelineBehaviorType)
    {
        ValidateCommandPipelineBehaviorType(commandPipelineBehaviorType);

        _serviceRegistrar.Register(typeof(ICommandPipelineBehavior<,>), commandPipelineBehaviorType);

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

    /// <summary>
    /// Validates that the specified type implements the ICommandPipelineBehavior interface.
    /// </summary>
    /// <param name="commandPipelineBehaviorType">The type to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the type does not implement ICommandPipelineBehavior interface.</exception>
    private static void ValidateCommandPipelineBehaviorType(Type commandPipelineBehaviorType)
    {
        // Check if the type implements any generic version of ICommandPipelineBehavior<,>
        var implementsICommandPipelineBehavior = commandPipelineBehaviorType
            .GetInterfaces()
            .Any(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(ICommandPipelineBehavior<,>));

        if (!implementsICommandPipelineBehavior)
        {
            throw new ArgumentException(
                $"Type '{commandPipelineBehaviorType.FullName}' must implement '{typeof(ICommandPipelineBehavior<,>).FullName}' interface.",
                nameof(commandPipelineBehaviorType));
        }

        // Ensure the type is not abstract and has a public constructor
        if (commandPipelineBehaviorType.IsAbstract)
        {
            throw new ArgumentException(
                $"Type '{commandPipelineBehaviorType.FullName}' cannot be abstract.",
                nameof(commandPipelineBehaviorType));
        }

        if (commandPipelineBehaviorType.IsInterface)
        {
            throw new ArgumentException(
                $"Type '{commandPipelineBehaviorType.FullName}' cannot be an interface.",
                nameof(commandPipelineBehaviorType));
        }
    }
}
