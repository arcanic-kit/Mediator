using Arcanic.Mediator.Abstractions.Modules;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Messaging.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arcanic.Mediator.Event;

public class EventModule: IModule
{
    private readonly Action<EventModuleBuilder> _builder;

    public EventModule(Action<EventModuleBuilder> builder)
    {
        _builder = builder;
    }

    public void Build(IServiceCollection services)
    {
        var messageRegistry = (MessageRegistryAccessor.Instance);

        _builder(new EventModuleBuilder(services, messageRegistry));

        services.TryAddTransient<IEventMediator, EventMediator>();
    }
}
