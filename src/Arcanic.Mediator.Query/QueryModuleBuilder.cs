using Arcanic.Mediator.Query.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Pipeline;

namespace Arcanic.Mediator.Query;

/// <summary>
/// Provides functionality for configuring and registering query handlers and related components
/// within the query mediator framework. This builder facilitates automatic discovery and registration
/// of query types and their corresponding handlers from assemblies.
/// </summary>
public class QueryModuleBuilder
{
    /// <summary>
    /// The service collection used for dependency injection registration.
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryModuleBuilder"/> class with the specified
    /// service collection and message registry.
    /// </summary>
    /// <param name="services">The dependency injection service collection used for registering discovered handlers.</param>
    public QueryModuleBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    /// Discovers and registers all query types and their corresponding handlers from the specified assembly.
    /// This method performs automatic registration by scanning for types that implement <see cref="IQuery{T}"/>
    /// and their associated handlers that implement <see cref="IQueryHandler{TQuery, TResult}"/>,
    /// <see cref="IQueryPreHandler{TQuery}"/>, or <see cref="IQueryPostHandler{TQuery}"/>.
    /// </summary>
    /// <param name="assembly">The assembly to scan for query types and handlers. All concrete classes
    /// implementing the appropriate interfaces will be registered automatically.</param>
    /// <returns>The current <see cref="QueryModuleBuilder"/> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a handler type cannot be properly analyzed.</exception>
    public QueryModuleBuilder RegisterFromAssembly(Assembly assembly)
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
            _services.AddTransient(registration.queryHandlerInterface, registration.handlerType);
        }
        
        _services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryPostHandlerPipelineBehavior<,>));
        _services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryPreHandlerPipelineBehavior<,>));

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
