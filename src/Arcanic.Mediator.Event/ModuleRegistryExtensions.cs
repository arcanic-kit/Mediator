using Arcanic.Mediator.Abstractions.Modules;
using Arcanic.Mediator.Messaging;

namespace Arcanic.Mediator.Event;

public static class ModuleRegistryExtensions
{
    public static IModuleRegistry AddEventModule(this IModuleRegistry moduleRegistry, Action<EventModuleBuilder> eventModuleBuilder)
    {
        ArgumentNullException.ThrowIfNull(moduleRegistry);
        ArgumentNullException.ThrowIfNull(eventModuleBuilder);

        //Ensure MessageModule is registered first with default configuration
        if (!moduleRegistry.IsModuleRegistered<MessageModule>())
        {
            moduleRegistry.Register(new MessageModule(_ => { }));
        }

        moduleRegistry.Register(new EventModule(eventModuleBuilder));

        return moduleRegistry;
    }
}