using Arcanic.Mediator.Abstractions.Modules;

namespace Arcanic.Mediator.Modules;

/// <summary>
/// Registry for managing and tracking registered <see cref="IModule"/> instances.
/// </summary>
public sealed class ModuleRegistry : IModuleRegistry
{
    private readonly HashSet<IModule> _modules = [];

    /// <summary>
    /// Gets the set of registered modules.
    /// </summary>
    /// <returns>A <see cref="HashSet{IModule}"/> containing all registered modules.</returns>
    public HashSet<IModule> GetModules() => _modules;

    /// <summary>
    /// Registers a module in the registry.
    /// </summary>
    /// <param name="module">The module to register.</param>
    /// <returns>The current <see cref="IModuleRegistry"/> instance.</returns>
    public IModuleRegistry Register(IModule module)
    {
        ArgumentNullException.ThrowIfNull(module);

        _modules.Add(module);

        return this;
    }

    /// <summary>
    /// Determines whether a module of type <typeparamref name="T"/> is registered.
    /// </summary>
    /// <typeparam name="T">The type of module to check.</typeparam>
    /// <returns><c>true</c> if the module is registered; otherwise, <c>false</c>.</returns>
    public bool IsModuleRegistered<T>() where T : IModule
    {
        return _modules.Any(module => module.GetType() == typeof(T));
    }
}
