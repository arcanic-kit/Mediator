using Arcanic.Mediator.Abstractions.Modules;
using Arcanic.Mediator.Messaging;

namespace Arcanic.Mediator.Query;

public static class ModuleRegistryExtensions
{
    public static IModuleRegistry AddQueryModule(this IModuleRegistry moduleRegistry, Action<QueryModuleBuilder> queryModuleBuilder)
    {
        ArgumentNullException.ThrowIfNull(moduleRegistry);
        ArgumentNullException.ThrowIfNull(queryModuleBuilder);

        //Ensure MessageModule is registered first with default configuration
        if (!moduleRegistry.IsModuleRegistered<MessageModule>())
        {
            moduleRegistry.Register(new MessageModule(_ => { }));
        }

        moduleRegistry.Register(new QueryModule(queryModuleBuilder));

        return moduleRegistry;
    }
}
