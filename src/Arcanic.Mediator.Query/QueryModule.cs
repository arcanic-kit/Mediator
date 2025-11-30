using Arcanic.Mediator.Abstractions.Modules;
using Arcanic.Mediator.Messaging.Registry;
using Arcanic.Mediator.Query.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arcanic.Mediator.Query;

/// <summary>
/// Provides a module for configuring query-based mediator services within the dependency injection container.
/// This module encapsulates the registration of query mediator components and their dependencies.
/// </summary>
public class QueryModule : IModule
{
    private readonly Action<QueryModuleBuilder> _builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryModule"/> class with the specified configuration action.
    /// </summary>
    /// <param name="builder">An action that configures the query module builder to register query handlers and related services.</param>
    public QueryModule(Action<QueryModuleBuilder> builder)
    {
        _builder = builder;
    }

    /// <summary>
    /// Configures the dependency injection container by registering query mediator services and executing the builder configuration.
    /// This method sets up the query infrastructure including the query mediator and processes any custom registrations
    /// specified through the builder action.
    /// </summary>
    /// <param name="services">The service collection to configure with query mediator registrations.</param>
    public void Build(IServiceCollection services)
    {
        var messageRegistry = (MessageRegistryAccessor.Instance);

        _builder(new QueryModuleBuilder(services, messageRegistry));

        services.TryAddScoped<IQueryMediator, QueryMediator>();
    }
}
