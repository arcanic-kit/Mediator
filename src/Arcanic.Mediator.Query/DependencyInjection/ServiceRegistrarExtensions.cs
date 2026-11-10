using Arcanic.Mediator.Abstractions.DependencyInjection;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Handler;
using Arcanic.Mediator.Query.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Pipeline;
using Arcanic.Mediator.DependencyInjection;
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
            .TryRegister(typeof(IQueryPipelineBehavior<,>), typeof(QueryPostHandlerPipelineBehavior<,>))
            .TryRegister(typeof(IQueryPipelineBehavior<,>), typeof(QueryPreHandlerPipelineBehavior<,>));

        return serviceRegistrar;
    }

    /// <summary>
    /// Discovers and registers all query types and their corresponding handlers from the specified assembly.
    /// This method performs automatic registration by scanning for types that implement <see cref="IQuery{T}"/>
    /// and their associated handlers that implement <see cref="IQueryHandler{TQuery, TResult}"/>,
    /// <see cref="IQueryPreHandler{TQuery}"/>, or <see cref="IQueryPostHandler{TQuery}"/>.
    /// </summary>
    /// <param name="serviceRegistrar">The service registrar to register the pipeline behavior with.</param>
    /// <param name="assembly">The assembly to scan for query types and handlers. All concrete classes
    /// implementing the appropriate interfaces will be registered automatically.</param>
    /// <returns>The current <see cref="IServiceRegistrar"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a handler type cannot be properly analyzed.</exception>
    public static IServiceRegistrar RegisterQueriesFromAssembly(this IServiceRegistrar serviceRegistrar, Assembly assembly) =>
        HandlerAssemblyScanner.RegisterHandlers(serviceRegistrar, assembly, IsQueryHandlerInterface, IsQueryInterface, "IQuery<T>");

    /// <summary>
    /// Registers a custom query pipeline behavior with the provided service registrar.
    /// The pipeline behavior will be executed as part of the query processing pipeline,
    /// allowing for cross-cutting concerns such as logging, validation, caching, or performance monitoring.
    /// </summary>
    /// <param name="serviceRegistrar">The service registrar to register the pipeline behavior with.</param>
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
        PipelineBehaviorValidator.Validate(queryPipelineBehaviorType, typeof(IQueryPipelineBehavior<,>), nameof(queryPipelineBehaviorType));

        serviceRegistrar.TryRegister(typeof(IQueryPipelineBehavior<,>), queryPipelineBehaviorType);

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
}
