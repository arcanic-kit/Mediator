using Arcanic.Mediator.Abstractions.Modules;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Messaging.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arcanic.Mediator.Command;

public class CommandModule: IModule
{
    private readonly Action<CommandModuleBuilder> _builder;

    public CommandModule(Action<CommandModuleBuilder> builder)
    {
        _builder = builder;
    }

    public void Build(IServiceCollection services)
    {
        var messageRegistry = (MessageRegistryAccessor.Instance);

        _builder(new CommandModuleBuilder(services, messageRegistry));

        services.TryAddTransient<ICommandMediator, CommandMediator>();
    }
}
