using Arcanic.Mediator.Abstractions.Modules;
using Arcanic.Mediator.Messaging.Registry;
using Arcanic.Mediator.Query.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arcanic.Mediator.Query;

public class QueryModule : IModule
{
    private readonly Action<QueryModuleBuilder> _builder;

    public QueryModule(Action<QueryModuleBuilder> builder)
    {
        _builder = builder;
    }

    public void Build(IServiceCollection services)
    {
        var messageRegistry = (MessageRegistryAccessor.Instance);

        _builder(new QueryModuleBuilder(services, messageRegistry));

        services.TryAddTransient<IQueryMediator, QueryMediator>();
    }
}
