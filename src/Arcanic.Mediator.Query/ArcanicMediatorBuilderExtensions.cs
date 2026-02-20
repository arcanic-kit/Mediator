using System.Reflection;
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Pipeline;
using Arcanic.Mediator.Query.DependencyInjection;
using Arcanic.Mediator.Request.DependencyInjection;

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
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(assembly);

        builder.ServiceRegistrar.RegisterRequestRequiredServices();

        builder.ServiceRegistrar.RegisterQueriesFromAssembly(assembly);
        builder.ServiceRegistrar.RegisterQueryRequiredServices();

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

        builder.ServiceRegistrar.RegisterQueryPipelineBehavior(queryPipelineBehaviorType);

        return builder;
    }
}