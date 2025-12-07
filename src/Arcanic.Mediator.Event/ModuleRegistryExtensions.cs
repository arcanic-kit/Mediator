using Arcanic.Mediator.Abstractions.Modules;
using Arcanic.Mediator.Messaging;

namespace Arcanic.Mediator.Event;

/// <summary>
/// Provides extension methods for <see cref="IModuleRegistry"/> to simplify the registration
/// of event-related modules within the mediator framework.
/// </summary>
public static class ModuleRegistryExtensions
{
    /// <summary>
    /// Registers an event module with the module registry, ensuring that the required message module
    /// dependency is also registered. This method provides a fluent interface for configuring
    /// event-based mediator functionality.
    /// </summary>
    /// <param name="moduleRegistry">The module registry to register the event module with.
    /// This parameter cannot be null.</param>
    /// <param name="eventModuleBuilder">An action that configures the event module builder
    /// to register event handlers and related services. This parameter cannot be null.</param>
    /// <returns>The module registry instance to enable method chaining for additional module registrations.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="moduleRegistry"/>
    /// or <paramref name="eventModuleBuilder"/> is null.</exception>
    public static IModuleRegistry AddEventModule(this IModuleRegistry moduleRegistry, Action<EventModuleBuilder> eventModuleBuilder)
    {
        ArgumentNullException.ThrowIfNull(moduleRegistry);
        ArgumentNullException.ThrowIfNull(eventModuleBuilder);

        //Ensure MessageModule is registered first with default configuration
        if (!moduleRegistry.IsModuleRegistered<MessageModule>())
        {
            moduleRegistry.Register(new MessageModule());
        }

        moduleRegistry.Register(new EventModule(eventModuleBuilder));

        return moduleRegistry;
    }
}