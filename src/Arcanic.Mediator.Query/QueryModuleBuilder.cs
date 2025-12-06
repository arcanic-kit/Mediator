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
    /// <summary>
    /// The service collection used for dependency injection registration.
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    /// The message registry used for mapping command types to their handlers.
    /// </summary>
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

        var concreteTypes = assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false, IsClass: true });

        foreach (var handlerType in concreteTypes)
        {
            var queryHandlerInterfaces = handlerType.GetInterfaces()
                .Where(IsQueryHandlerInterface)
                .ToList();

            if (queryHandlerInterfaces.Count == 0)
                continue;

            foreach (var queryHandlerInterface in queryHandlerInterfaces)
            {
                var queryType = queryHandlerInterface.GetGenericArguments().FirstOrDefault();
                
                if (queryType == null)
                {
                    throw new InvalidOperationException(
                        $"Unable to determine query type for handler '{handlerType.FullName}'. " +
                        $"The handler interface '{queryHandlerInterface.FullName}' does not have generic arguments.");
                }

                // Ensure the query type implements IQuery<T>
                if (!queryType.GetInterfaces().Any(IsQueryInterface))
                {
                    throw new InvalidOperationException(
                        $"Query type '{queryType.FullName}' handled by '{handlerType.FullName}' must implement IQuery<T> interface.");
                }

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
