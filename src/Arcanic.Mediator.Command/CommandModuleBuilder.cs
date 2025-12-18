using Arcanic.Mediator.Command.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Arcanic.Mediator.Command.Abstractions.Handler;

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
    /// Initializes a new instance of the <see cref="CommandModuleBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection used for dependency injection registration.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public CommandModuleBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
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
            _services.AddTransient(registration.commandHandlerInterface, registration.handlerType);
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
