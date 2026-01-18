using System;
using System.Linq;
using System.Reflection;
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Query;

/// <summary>
/// Provides extension methods for <see cref="IArcanicMediatorBuilder"/> to register query handlers and related services.
/// </summary>
public static class ArcanicMediatorBuilderExtensions
{
    /// <summary>
    /// Registers all query handlers found in the specified assembly along with required query services.
    /// This method scans the assembly for query implementations and registers them with the dependency injection container.
    /// </summary>
    /// <param name="builder">The mediator builder instance to extend.</param>
    /// <param name="assembly">The assembly to scan for query handlers.</param>
    /// <returns>The mediator builder instance to enable method chaining.</returns>
    public static IArcanicMediatorBuilder AddQueries(this IArcanicMediatorBuilder builder, Assembly assembly)
    {
        var queryServiceRegistrar = new QueryServiceRegistrar(builder.Services, builder.Configuration);

        queryServiceRegistrar.RegisterQueriesFromAssembly(assembly);
        queryServiceRegistrar.RegisterRequiredServices();

        return builder;
    }
    
    /// <summary>
    /// Registers a custom pipeline behavior for query processing in the mediator pipeline.
    /// Pipeline behaviors allow cross-cutting concerns such as logging, validation, caching, 
    /// or transaction management to be applied to query handling.
    /// </summary>
    /// <param name="builder">The mediator builder instance to extend.</param>
    /// <param name="queryPipelineBehaviorType">
    /// The type of the pipeline behavior to register. Must implement <see cref="IQueryPipelineBehavior{TQuery, TQueryResult}"/>.
    /// </param>
    /// <returns>The mediator builder instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="builder"/> or <paramref name="queryPipelineBehaviorType"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="queryPipelineBehaviorType"/> does not implement the required interface,
    /// is abstract, or is an interface type.
    /// </exception>
    public static IArcanicMediatorBuilder AddQueryPipelineBehavior(this IArcanicMediatorBuilder builder,
        Type queryPipelineBehaviorType)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(queryPipelineBehaviorType);
        
        ValidateQueryPipelineBehaviorType(queryPipelineBehaviorType);
        
        builder.Services.Add(new ServiceDescriptor(typeof(IQueryPipelineBehavior<,>), queryPipelineBehaviorType, builder.Configuration.InstanceLifetime));
        
        return builder;
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