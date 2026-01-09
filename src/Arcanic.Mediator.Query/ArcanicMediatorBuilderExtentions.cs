using System.Reflection;
using Arcanic.Mediator.Abstractions;

namespace Arcanic.Mediator.Query;

public static class ArcanicMediatorBuilderExtentions
{
    public static IArcanicMediatorBuilder AddQueries(this IArcanicMediatorBuilder builder, Assembly assembly)
    {
        var queryModuleBuilder = new QueryModuleBuilder(builder.Services);
        
        queryModuleBuilder.RegisterQueriesFromAssembly(assembly);
        queryModuleBuilder.RegisterRequiredServices();
        
        return builder;
    }
}