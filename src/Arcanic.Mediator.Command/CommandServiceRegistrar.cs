using System;
using System.Linq;
using Arcanic.Mediator.Command.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Configuration;
using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Command.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Pipeline;

namespace Arcanic.Mediator.Command;

/// <summary>
/// Provides functionality for registering command-related services and handlers with the dependency injection container.
/// This class handles automatic discovery and registration of command handlers, pre-handlers, post-handlers,
/// and required pipeline services from assemblies.
/// </summary>
public class CommandServiceRegistrar
{
    /// <summary>
    /// The configuration settings for the Arcanic Mediator service, including service lifetime options.
    /// </summary>
    private readonly ArcanicMediatorServiceConfiguration _configuration;

    /// <summary>
    /// The service collection used for dependency injection registration.
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandServiceRegistrar"/> class.
    /// </summary>
    /// <param name="services">The service collection used for dependency injection registration.</param>
    /// <param name="configuration">The configuration settings for the Arcanic Mediator service.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public CommandServiceRegistrar(IServiceCollection services, ArcanicMediatorServiceConfiguration configuration)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Registers the core services required for command processing, including the command mediator
    /// and default pipeline behaviors for pre and post-processing.
    /// </summary>
    /// <returns>The current <see cref="CommandServiceRegistrar"/> instance to enable method chaining.</returns>
    public CommandServiceRegistrar RegisterRequiredServices()
    {
        _services.Add(new ServiceDescriptor(typeof(ICommandMediator), typeof(CommandMediator), _configuration.InstanceLifetime));
        _services.Add(new ServiceDescriptor(typeof(ICommandPipelineBehavior<,>), typeof(CommandPostHandlerPipelineBehavior<,>), _configuration.InstanceLifetime));
        _services.Add(new ServiceDescriptor(typeof(ICommandPipelineBehavior<,>), typeof(CommandPreHandlerPipelineBehavior<,>), _configuration.InstanceLifetime));

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
