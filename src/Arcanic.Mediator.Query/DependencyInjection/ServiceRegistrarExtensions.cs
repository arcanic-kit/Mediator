using Arcanic.Mediator.Abstractions.DependencyInjection;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Handler;
using Arcanic.Mediator.Query.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Pipeline;
using System.Reflection;

namespace Arcanic.Mediator.Query.DependencyInjection;

/// <summary>
/// Provides extension methods for <see cref="IServiceRegistrar"/> to simplify registration of query-related services and handlers.
/// These extensions enable fluent configuration of query processing components including mediators, handlers, and pipeline behaviors.
/// </summary>
public static class ServiceRegistrarExtensions
{
    /// <summary>
    /// Registers the core services required for query processing, including the query mediator
    /// and default pipeline behaviors for pre and post-processing.
    /// </summary>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    public static IServiceRegistrar RegisterQueryRequiredServices(this IServiceRegistrar serviceRegistrar)
    {
        serviceRegistrar
            .Register(typeof(IQueryMediator), typeof(QueryMediator))
            .Register(typeof(IQueryPipelineBehavior<,>), typeof(QueryPostHandlerPipelineBehavior<,>))
            .Register(typeof(IQueryPipelineBehavior<,>), typeof(QueryPreHandlerPipelineBehavior<,>));

        return serviceRegistrar;
    }

    /// <summary>
    /// Discovers and registers all query types and their corresponding handlers from the specified assembly.
    /// This method performs automatic registration by scanning for types that implement <see cref="IQuery{T}"/>
    /// and their associated handlers that implement <see cref="IQueryHandler{TQuery, TResult}"/>,
    /// <see cref="IQueryPreHandler{TQuery}"/>, or <see cref="IQueryPostHandler{TQuery}"/>.
    /// </summary>
    /// <param name="assembly">The assembly to scan for query types and handlers. All concrete classes
    /// implementing the appropriate interfaces will be registered automatically.</param>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a handler type cannot be properly analyzed.</exception>
    public static IServiceRegistrar RegisterQueriesFromAssembly(this IServiceRegistrar serviceRegistrar, Assembly assembly)
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
            serviceRegistrar.Register(registration.queryHandlerInterface, registration.handlerType);
        }

        return serviceRegistrar;
    }

    /// <summary>
    /// Registers a custom query pipeline behavior with the dependency injection container.
    /// The pipeline behavior will be executed as part of the query processing pipeline,
    /// allowing for cross-cutting concerns such as logging, validation, caching, or performance monitoring.
    /// </summary>
    /// <param name="queryPipelineBehaviorType">
    /// The type that implements <see cref="IQueryPipelineBehavior{TQuery, TQueryResult}"/>.
    /// Must be a concrete class that is not abstract or an interface.
    /// </param>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="queryPipelineBehaviorType"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="queryPipelineBehaviorType"/> does not implement the required 
    /// <see cref="IQueryPipelineBehavior{TQuery, TQueryResult}"/> interface, is abstract, or is an interface type.
    /// </exception>
    public static IServiceRegistrar RegisterQueryPipelineBehavior(this IServiceRegistrar serviceRegistrar, Type queryPipelineBehaviorType)
    {
        ValidateQueryPipelineBehaviorType(queryPipelineBehaviorType);

        serviceRegistrar.Register(typeof(IQueryPipelineBehavior<,>), queryPipelineBehaviorType);

        return serviceRegistrar;
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

    /// <summary>
    /// Validates that the provided type is suitable for use as a query pipeline behavior.
    /// This method ensures the type implements the required interface, is not abstract, and is not an interface.
    /// </summary>
    /// <param name="queryPipelineBehaviorType">The type to validate for query pipeline behavior compatibility.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the type does not meet the requirements:
    /// - Must implement <see cref="IQueryPipelineBehavior{TQuery, TQueryResult}"/>
    /// - Cannot be abstract
    /// - Cannot be an interface
    /// </exception>
    private static void ValidateQueryPipelineBehaviorType(Type queryPipelineBehaviorType)
    {
        // Check if the type implements any generic version of IQueryPipelineBehavior<,>
        var implementsIQueryPipelineBehavior = queryPipelineBehaviorType
            .GetInterfaces()
            .Any(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IQueryPipelineBehavior<,>));

        if (!implementsIQueryPipelineBehavior)
        {
            throw new ArgumentException(
                $"Type '{queryPipelineBehaviorType.FullName}' must implement '{typeof(IQueryPipelineBehavior<,>).FullName}' interface.",
                nameof(queryPipelineBehaviorType));
        }

        // Ensure the type is not abstract and has a public constructor
        if (queryPipelineBehaviorType.IsAbstract)
        {
            throw new ArgumentException(
                $"Type '{queryPipelineBehaviorType.FullName}' cannot be abstract.",
                nameof(queryPipelineBehaviorType));
        }

        if (queryPipelineBehaviorType.IsInterface)
        {
            throw new ArgumentException(
                $"Type '{queryPipelineBehaviorType.FullName}' cannot be an interface.",
                nameof(queryPipelineBehaviorType));
        }
    }
}
