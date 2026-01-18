using Arcanic.Mediator.Query.Abstractions;
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
public class QueryDependencyRegistry
{
    /// <summary>
    /// Lazy singleton accessor for the DependencyRegistry instance.
    /// </summary>
    private readonly DependencyRegistryAccessor _dependencyRegistryAccessor;

    public QueryDependencyRegistry(DependencyRegistryAccessor dependencyRegistryAccessor)
    {
        _dependencyRegistryAccessor = dependencyRegistryAccessor;
    }

    /// <summary>
    /// Registers the core services required for query processing, including the query mediator
    /// and default pipeline behaviors for pre and post-processing.
    /// </summary>
    /// <returns>The current <see cref="QueryDependencyRegistry"/> instance to enable method chaining.</returns>
    public QueryDependencyRegistry RegisterRequiredServices()
    {
        _dependencyRegistryAccessor.Registry
            .Add(typeof(IQueryMediator), typeof(QueryMediator))
            .Add(typeof(IQueryPipelineBehavior<,>), typeof(QueryPostHandlerPipelineBehavior<,>))
            .Add(typeof(IQueryPipelineBehavior<,>), typeof(QueryPreHandlerPipelineBehavior<,>));

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
    /// <returns>The current <see cref="QueryDependencyRegistry"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a handler type cannot be properly analyzed.</exception>
    public QueryDependencyRegistry RegisterQueriesFromAssembly(Assembly assembly)
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
            _dependencyRegistryAccessor.Registry.Add(registration.queryHandlerInterface, registration.handlerType);
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
