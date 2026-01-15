using System;
using System.Linq;
using System.Reflection;
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Command;

/// <summary>
/// Provides extension methods for <see cref="IArcanicMediatorBuilder"/> to enable command functionality.
/// </summary>
public static class ArcanicMediatorBuilderExtensions
{
    /// <summary>
    /// Adds command functionality to the Arcanic Mediator by registering command handlers and required services.
    /// This method scans the specified assembly for command types and their corresponding handlers 
    /// (main, pre, and post handlers), then registers them with the dependency injection container
    /// along with the core command processing services.
    /// </summary>
    /// <param name="builder">The <see cref="IArcanicMediatorBuilder"/> instance to extend with command functionality.</param>
    /// <param name="assembly">The assembly to scan for command implementations and handlers.</param>
    /// <returns>The same <see cref="IArcanicMediatorBuilder"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="assembly"/> is null.</exception>
    public static IArcanicMediatorBuilder AddCommands(this IArcanicMediatorBuilder builder, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(assembly);
        
        var commandServiceRegistrar = new CommandServiceRegistrar(builder.Services, builder.Configuration);

        commandServiceRegistrar.RegisterCommandsFromAssembly(assembly);
        commandServiceRegistrar.RegisterRequiredServices();

        return builder;
    }

    /// <summary>
    /// Adds a command-specific pipeline behavior to the mediator configuration.
    /// Pipeline behaviors allow cross-cutting concerns to be applied specifically to command handling.
    /// </summary>
    /// <param name="builder">The <see cref="IArcanicMediatorBuilder"/> instance to extend.</param>
    /// <param name="commandPipelineBehaviorType">
    /// The type that implements the command pipeline behavior interface.
    /// Must implement <see cref="ICommandPipelineBehavior{TCommand, TCommandResponse}"/> interface.
    /// </param>
    /// <returns>The same <see cref="IArcanicMediatorBuilder"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="commandPipelineBehaviorType"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="commandPipelineBehaviorType"/> does not implement the required interface, is abstract, or is an interface type.</exception>
    public static IArcanicMediatorBuilder AddCommandPipelineBehavior(this IArcanicMediatorBuilder builder,
        Type commandPipelineBehaviorType)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(commandPipelineBehaviorType);
        
        ValidateCommandPipelineBehaviorType(commandPipelineBehaviorType);
        
        builder.Services.Add(new ServiceDescriptor(typeof(ICommandPipelineBehavior<,>), commandPipelineBehaviorType, builder.Configuration.Lifetime));
        
        return builder;
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