using Arcanic.Mediator.Abstractions.Modules;

namespace Arcanic.Mediator.Query;

/// <summary>
/// Provides extension methods for <see cref="IModuleRegistry"/> to simplify the registration
/// of query-related modules within the mediator framework.
/// </summary>
public static class ModuleRegistryExtensions
{
    /// <summary>
    /// Registers a query module with the module registry, ensuring that the required message module
    /// dependency is also registered.
    /// </summary>
    /// <param name="moduleRegistry">The module registry to register the query module with.</param>
    /// <param name="queryModuleBuilder">An action that configures the query module builder
    /// to register query handlers and related services.</param>
    /// <returns>The module registry instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="moduleRegistry"/>
    /// or <paramref name="queryModuleBuilder"/> is null.</exception>
    public static IModuleRegistry AddQueryModule(this IModuleRegistry moduleRegistry, Action<QueryModuleBuilder> queryModuleBuilder)
    {
        ArgumentNullException.ThrowIfNull(moduleRegistry);
        ArgumentNullException.ThrowIfNull(queryModuleBuilder);
        
        moduleRegistry.Register(new QueryModule(queryModuleBuilder));

        return moduleRegistry;
    }
}
