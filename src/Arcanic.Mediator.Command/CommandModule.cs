using Arcanic.Mediator.Abstractions.Modules;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arcanic.Mediator.Command;

/// <summary>
/// A module that configures command processing services and handlers in the dependency injection container.
/// This module enables command mediation functionality by registering the command mediator and any configured command handlers.
/// </summary>
public class CommandModule: IModule
{
    /// <summary>
    /// The builder action used to configure command registrations and services.
    /// </summary>
    private readonly Action<CommandModuleBuilder> _builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandModule"/> class.
    /// </summary>
    /// <param name="builder">An action that configures the command module using the provided builder.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is null.</exception>
    public CommandModule(Action<CommandModuleBuilder> builder)
    {
        _builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    /// <summary>
    /// Configures the dependency injection container by registering command-related services and handlers.
    /// This method sets up the command mediator and executes the configured builder action to register
    /// any additional command handlers or services.
    /// </summary>
    /// <param name="services">The service collection to configure with command processing services.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public void Build(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        _builder(new CommandModuleBuilder(services));

        services.TryAddTransient<ICommandMediator, CommandMediator>();
        
        services.AddTransient(typeof(ICommandPipelineBehavior<,>), typeof(CommandPostHandlerPipelineBehavior<,>));
        services.AddTransient(typeof(ICommandPipelineBehavior<,>), typeof(CommandPreHandlerPipelineBehavior<,>));
    }
}
