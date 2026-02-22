using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Pipeline;
using Arcanic.Mediator.Request.Abstractions.Dispatcher;
using Arcanic.Mediator.Request.Abstractions.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Query.Dispatcher;

/// <summary>
/// Abstract base class for query dispatchers.
/// Provides an interface for dispatching queries with dynamic query types and dependency injection.
/// </summary>
public abstract class QueryDispatcherBase : RequestDispatcherBase
{    
    /// <summary>
    /// Retrieves all pipeline behaviors for the specified query and response types from the service provider.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query that implements <see cref="IQuery{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
    /// <param name="serviceProvider">The service provider used to resolve pipeline behavior services.</param>
    /// <returns>
    /// An enumerable collection of pipeline behaviors in reverse order of registration, 
    /// including query-specific, request-specific, and generic pipeline behaviors.
    /// </returns>
    protected IEnumerable<IPipelineBehavior<TQuery, TResponse>> GetAllPipelineBehaviors<TQuery, TResponse>(IServiceProvider serviceProvider)
        where TQuery : IQuery<TResponse>
    {
        // Retrieve query-specific pipeline behaviors in reverse order
        var queryPipelineBehaviors = serviceProvider
            .GetServices<IQueryPipelineBehavior<TQuery, TResponse>>()
            .Reverse();
        
        // Retrieve request-specific pipeline behaviors in reverse order
        var requestedPipelineBehaviors = serviceProvider
            .GetServices<IRequestPipelineBehavior<TQuery, TResponse>>()
            .Reverse();
        
        // Retrieve generic pipeline behaviors in reverse order
        var pipelineBehaviors = serviceProvider
            .GetServices<IPipelineBehavior<TQuery, TResponse>>()
            .Reverse();

        // Combine all pipeline behaviors in the correct execution order
        return queryPipelineBehaviors
            .Cast<IPipelineBehavior<TQuery, TResponse>>()
            .Concat(requestedPipelineBehaviors)
            .Concat(pipelineBehaviors);
    }
}