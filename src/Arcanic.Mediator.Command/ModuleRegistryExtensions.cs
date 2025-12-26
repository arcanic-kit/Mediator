using Arcanic.Mediator.Abstractions.Modules;

namespace Arcanic.Mediator.Command;

/// <summary>
/// Provides extension methods for the module registry to support command module registration.
/// These extensions enable fluent configuration of command-based messaging capabilities within the mediator framework.
/// </summary>
public static class ModuleRegistryExtensions
{
    /// <summary>
    /// Adds a command module to the module registry with the specified configuration.
    /// This method ensures that the required MessageModule dependency is registered before
    /// adding the CommandModule, maintaining proper module initialization order.
    /// </summary>
    /// <param name="moduleRegistry">The module registry to which the command module will be added.</param>
    /// <param name="commandModuleBuilder">An action that configures the command module by registering command types and their handlers.</param>
    /// <returns>The module registry instance to support fluent chaining of module registrations.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="moduleRegistry"/> or <paramref name="commandModuleBuilder"/> is null.</exception>
    public static IModuleRegistry AddCommandModule(this IModuleRegistry moduleRegistry, Action<CommandModuleBuilder> commandModuleBuilder)
    {
        ArgumentNullException.ThrowIfNull(moduleRegistry);
        ArgumentNullException.ThrowIfNull(commandModuleBuilder);

        moduleRegistry.Register(new CommandModule(commandModuleBuilder));

        return moduleRegistry;
    }
}
