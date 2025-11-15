
namespace Arcanic.Mediator.Abstractions.Modules;

/// <summary>
/// Defines a registry for managing and organizing modules within the mediator framework.
/// The registry provides functionality to register modules, retrieve them, and check for their presence.
/// </summary>
public interface IModuleRegistry
{
    /// <summary>
    /// Registers a module with the registry for later use during service configuration.
    /// </summary>
    /// <param name="module">The module to register with the registry.</param>
    /// <returns>The current module registry instance to support fluent registration chains.</returns>
    IModuleRegistry Register(IModule module);

    /// <summary>
    /// Retrieves all registered modules from the registry.
    /// </summary>
    /// <returns>A hash set containing all registered modules in the registry.</returns>
    HashSet<IModule> GetModules();

    /// <summary>
    /// Determines whether a module of the specified type is already registered in the registry.
    /// </summary>
    /// <typeparam name="T">The type of module to check for registration.</typeparam>
    /// <returns>True if a module of the specified type is registered; otherwise, false.</returns>
    bool IsModuleRegistered<T>() where T : IModule;
}
