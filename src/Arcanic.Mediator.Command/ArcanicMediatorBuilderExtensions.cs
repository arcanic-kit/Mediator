using System.Reflection;
using Arcanic.Mediator.Abstractions;

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
        var commandServiceRegistrar = new CommandServiceRegistrar(builder.Services, builder.Configuration);

        commandServiceRegistrar.RegisterCommandsFromAssembly(assembly);
        commandServiceRegistrar.RegisterRequiredServices();

        return builder;
    }
}