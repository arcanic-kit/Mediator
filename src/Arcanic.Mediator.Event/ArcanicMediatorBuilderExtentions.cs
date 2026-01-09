using System.Reflection;
using Arcanic.Mediator.Abstractions;

namespace Arcanic.Mediator.Event;

public static class ArcanicMediatorBuilderExtentions
{
    public static IArcanicMediatorBuilder AddEvents(this IArcanicMediatorBuilder builder, Assembly assembly)
    {
        var eventModuleBuilder = new EventModuleBuilder(builder.Services);
        
        eventModuleBuilder.RegisterEventsFromAssembly(assembly);
        eventModuleBuilder.RegisterRequiredServices();

        return builder;
    }
}