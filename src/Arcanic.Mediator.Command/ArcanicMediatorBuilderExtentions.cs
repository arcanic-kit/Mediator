using System.Reflection;
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arcanic.Mediator.Command;

public static class ArcanicMediatorBuilderExtentions
{
    public static IArcanicMediatorBuilder AddCommands(this IArcanicMediatorBuilder builder, Assembly assembly)
    {
        var commandModuleBuilder = new CommandModuleBuilder(builder.Services);
        commandModuleBuilder.RegisterCommandsFromAssembly(assembly);
        commandModuleBuilder.RegisterRequiredServices();

        return builder;
    }
}