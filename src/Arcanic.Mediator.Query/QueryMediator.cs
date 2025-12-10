using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Mediator;
using Arcanic.Mediator.Messaging.Mediator.Strategies;
using Arcanic.Mediator.Query.Abstractions;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Query;

/// <summary>
/// Provides query mediation capabilities by coordinating the execution of queries
/// through the underlying message mediator framework.
/// High-performance version that includes fast paths for common scenarios.
/// </summary>
public class QueryMediator : IQueryMediator
{
    /// <summary>
    /// The underlying message mediator responsible for coordinating query processing and handler invocation.
    /// </summary>
    private readonly IMessageMediator _messageMediator;

    /// <summary>
    /// The service provider used for dependency injection and handler resolution.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;
    
    /// <summary>
    /// Cache for direct query strategies to avoid repeated allocations.
    /// </summary>
    private readonly ConcurrentDictionary<Type, object> _directStrategyCache = new();
    
    /// <summary>
    /// Cache for pipeline query strategies to avoid repeated allocations.
    /// </summary>
    private readonly ConcurrentDictionary<Type, object> _pipelineStrategyCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryMediator"/> class with the specified message mediator.
    /// </summary>
    /// <param name="messageMediator">The message mediator instance used for coordinating query processing.</param>
    /// <param name="serviceProvider">The service provider used for dependency injection and handler resolution.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageMediator"/> or <paramref name="serviceProvider"/> is null.</exception>
    public QueryMediator(IMessageMediator messageMediator, IServiceProvider serviceProvider)
    {
        _messageMediator = messageMediator ?? throw new ArgumentNullException(nameof(messageMediator));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Asynchronously executes a query and returns the result using a single handler strategy.
    /// Uses fast path optimization when no pipeline behaviors are registered.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of result returned by the query.</typeparam>
    /// <param name="query">The query to execute.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous query execution with the result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is null.</exception>
    public async Task<TQueryResult> SendAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var queryType = query.GetType();
        
        // Check if we can use the fast path (no pipeline behaviors)
        var behaviorType = typeof(Messaging.Abstractions.Pipeline.IRequestPipelineBehavior<,>).MakeGenericType(typeof(IQuery<TQueryResult>), typeof(TQueryResult));
        var hasBehaviors = _serviceProvider.GetServices(behaviorType).Any();
        
        if (!hasBehaviors)
        {
            // Fast path: use direct handler strategy
            var directStrategy = (MessageMediatorDirectHandlerStrategy<IQuery<TQueryResult>, TQueryResult>)
                _directStrategyCache.GetOrAdd(queryType, 
                    _ => new MessageMediatorDirectHandlerStrategy<IQuery<TQueryResult>, TQueryResult>());

            var directOptions = new MessageMediatorOptions<IQuery<TQueryResult>, Task<TQueryResult>>()
            {
                Strategy = directStrategy,
                CancellationToken = cancellationToken,
            };

            return await _messageMediator.Mediate(query, directOptions);
        }
        
        // Standard path: use pipeline strategy
        var strategy = (MessageMediatorRequestPipelineHandlerStrategy<IQuery<TQueryResult>, TQueryResult>)
            _pipelineStrategyCache.GetOrAdd(queryType, 
                _ => new MessageMediatorRequestPipelineHandlerStrategy<IQuery<TQueryResult>, TQueryResult>(_serviceProvider));

        var options = new MessageMediatorOptions<IQuery<TQueryResult>, Task<TQueryResult>>()
        {
            Strategy = strategy,
            CancellationToken = cancellationToken,
        };

        return await _messageMediator.Mediate(query, options);
    }
}
