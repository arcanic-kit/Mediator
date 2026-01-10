using Arcanic.Mediator.Query.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Handler;
using Arcanic.Mediator.Query.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Pipeline;

namespace Arcanic.Mediator.Query;

/// <summary>
/// Provides functionality for registering query-related services and handlers with the dependency injection container.
/// This class handles automatic discovery and registration of query handlers, pre-handlers, post-handlers,
/// and required pipeline services from assemblies.
/// </summary>
public class QueryServiceRegistrar
{
    /// <summary>
    /// The configuration settings for the Arcanic Mediator service, including service lifetime options.
    /// </summary>
    private readonly ArcanicMediatorServiceConfiguration _configuration;

    /// <summary>
    /// The service collection used for dependency injection registration.
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryServiceRegistrar"/> class with the specified
    /// service collection and message registry.
    /// </summary>
    /// <param name="services">The dependency injection service collection used for registering discovered handlers.</param>
    /// <param name="configuration">The configuration settings for the Arcanic Mediator service.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    public QueryServiceRegistrar(IServiceCollection services, ArcanicMediatorServiceConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    /// Registers the core services required for query processing, including the query mediator
    /// and default pipeline behaviors for pre and post-processing.
    /// </summary>
    /// <returns>The current <see cref="QueryServiceRegistrar"/> instance to enable method chaining.</returns>
    public QueryServiceRegistrar RegisterRequiredServices()
    {
        _services.Add(new ServiceDescriptor(typeof(IQueryMediator), typeof(QueryMediator), _configuration.Lifetime));
        _services.Add(new ServiceDescriptor(typeof(IQueryPipelineBehavior<,>), typeof(QueryPostHandlerPipelineBehavior<,>), _configuration.Lifetime));
        _services.Add(new ServiceDescriptor(typeof(IQueryPipelineBehavior<,>), typeof(QueryPreHandlerPipelineBehavior<,>), _configuration.Lifetime));

        return this;
    }

    /// <summary>
    /// Discovers and registers all query types and their corresponding handlers from the specified assembly.
    /// This method performs automatic registration by scanning for types that implement <see cref="IQuery{T}"/>
    /// and their associated handlers that implement <see cref="IQueryHandler{TQuery, TResult}"/>,
    /// <see cref="IQueryPreHandler{TQuery}"/>, or <see cref="IQueryPostHandler{TQuery}"/>.
    /// </summary>
    /// <param name="assembly">The assembly to scan for query types and handlers. All concrete classes
    /// implementing the appropriate interfaces will be registered automatically.</param>
    /// <returns>The current <see cref="QueryServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a handler type cannot be properly analyzed.</exception>
    public QueryServiceRegistrar RegisterQueriesFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var queryHandlerRegistrations = assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false, IsClass: true })
            .SelectMany(handlerType => handlerType.GetInterfaces()
                .Where(IsQueryHandlerInterface)
                .Select(queryHandlerInterface => new { handlerType, queryHandlerInterface }))
            .Where(registration =>
            {
                var queryType = registration.queryHandlerInterface.GetGenericArguments().FirstOrDefault();

                if (queryType == null)
                {
                    throw new InvalidOperationException(
                        $"Unable to determine query type for handler '{registration.handlerType.FullName}'. " +
                        $"The handler interface '{registration.queryHandlerInterface.FullName}' does not have generic arguments.");
                }

                if (!queryType.GetInterfaces().Any(IsQueryInterface))
                {
                    throw new InvalidOperationException(
                        $"Query type '{queryType.FullName}' handled by '{registration.handlerType.FullName}' must implement IQuery<T> interface.");
                }

                return true;
            });

        foreach (var registration in queryHandlerRegistrations)
        {
            _services.Add(new ServiceDescriptor(registration.queryHandlerInterface, registration.handlerType, _configuration.Lifetime));
        }

        return this;
    }

    /// <summary>
    /// Determines whether the specified type represents a query interface.
    /// This method checks if the type is a generic interface with the type definition <see cref="IQuery{T}"/>.
    /// </summary>
    /// <param name="type">The type to examine for query interface compatibility.</param>
    /// <returns>True if the type is a generic <see cref="IQuery{T}"/> interface; otherwise, false.</returns>
    private static bool IsQueryInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();
        return genericTypeDefinition == typeof(IQuery<>);
    }

    /// <summary>
    /// Determines whether the specified type represents a query handler interface (main, pre, or post).
    /// This method checks if the type is a generic interface with the type definition <see cref="IQueryHandler{TQuery, TResult}"/>,
    /// <see cref="IQueryPreHandler{TQuery}"/>, or <see cref="IQueryPostHandler{TQuery}"/>.
    /// </summary>
    /// <param name="type">The type to examine for query handler interface compatibility.</param>
    /// <returns>True if the type is a query handler interface; otherwise, false.</returns>
    private static bool IsQueryHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();

        return genericTypeDefinition == typeof(IQueryHandler<,>) ||
               genericTypeDefinition == typeof(IQueryPreHandler<>) ||
               genericTypeDefinition == typeof(IQueryPostHandler<>);
    }
}
