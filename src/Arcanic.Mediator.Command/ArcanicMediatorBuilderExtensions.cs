using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Pipeline;
using Arcanic.Mediator.Command.DependencyInjection;
using System.Reflection;

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

        builder.ServiceRegistrar.RegisterCommandsFromAssembly(assembly);
        builder.ServiceRegistrar.RegisterCommandRequiredServices();

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

        builder.ServiceRegistrar.RegisterCommandPipelineBehavior(commandPipelineBehaviorType);

        return builder;
    }
}