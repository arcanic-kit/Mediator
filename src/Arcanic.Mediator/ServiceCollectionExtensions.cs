using Arcanic.Mediator.Abstractions.Modules;
using Arcanic.Mediator.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator;

/// <summary>
/// Extension methods for registering Arcanic Mediator modules with the <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Arcanic Mediator modules to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add modules to.</param>
    /// <param name="moduleRegistryAction">An action to register modules using the <see cref="IModuleRegistry"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddArcanicMediator(this IServiceCollection services, Action<IModuleRegistry> moduleRegistryAction)
    {
        ArgumentNullException.ThrowIfNull(moduleRegistryAction);

        var moduleRegistry = new ModuleRegistry();
        moduleRegistryAction(moduleRegistry);

        foreach (var modules in moduleRegistry.GetModules())
        {
            modules.Build(services);
        }

        return services;
    }
}
