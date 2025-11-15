using Arcanic.Mediator.Messaging.Abstractions.Registry;
using Arcanic.Mediator.Query.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Query;

/// <summary>
/// Provides functionality for configuring and registering query handlers and related components
/// within the query mediator framework. This builder facilitates automatic discovery and registration
/// of query types and their corresponding handlers from assemblies.
/// </summary>
public class QueryModuleBuilder
{
    private readonly IServiceCollection _services;
    private readonly IMessageRegistry _messageRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryModuleBuilder"/> class with the specified
    /// service collection and message registry.
    /// </summary>
    /// <param name="services">The dependency injection service collection used for registering discovered handlers.</param>
    /// <param name="messageRegistry">The message registry used for mapping query types to their handlers.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="services"/> or 
    /// <paramref name="messageRegistry"/> is null.</exception>
    public QueryModuleBuilder(IServiceCollection services, IMessageRegistry messageRegistry)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _messageRegistry = messageRegistry ?? throw new ArgumentNullException(nameof(messageRegistry));
    }

    /// <summary>
    /// Discovers and registers all query types and their corresponding handlers from the specified assembly.
    /// This method performs automatic registration by scanning for types that implement <see cref="IQuery{T}"/>
    /// and their associated handlers that implement <see cref="IQueryHandler{TQuery, TResult}"/>.
    /// </summary>
    /// <param name="assembly">The assembly to scan for query types and handlers.</param>
    /// <returns>The current <see cref="QueryModuleBuilder"/> instance to enable method chaining.</returns>
    public QueryModuleBuilder RegisterFromAssembly(Assembly assembly)
    {
        var queryTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass)
            .Where(t => t.GetInterfaces().Any(IsQueryInterface))
            .ToList();

        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass)
            .Where(t => t.GetInterfaces().Any(IsQueryHandlerInterface))
            .ToList();

        foreach (var queryType in queryTypes)
        {
            var matchingHandlers = handlerTypes.Where(handlerType =>
                handlerType.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)) &&
                    i.GetGenericArguments()[0] == queryType))
                .ToList();

            // Register each matching handler
            foreach (var handlerType in matchingHandlers)
            {
                _messageRegistry.Register(queryType, handlerType);
                _services.AddScoped(handlerType);
            }
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
    /// Determines whether the specified type represents a query handler interface.
    /// This method checks if the type is a generic interface with the type definition <see cref="IQueryHandler{TQuery, TResult}"/>.
    /// </summary>
    /// <param name="type">The type to examine for query handler interface compatibility.</param>
    /// <returns>True if the type is a generic <see cref="IQueryHandler{TQuery, TResult}"/> interface; otherwise, false.</returns>
    private static bool IsQueryHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();

        return genericTypeDefinition == typeof(IQueryHandler<,>);
    }
}
