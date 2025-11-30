using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Mediator;
using Arcanic.Mediator.Messaging.Mediator.Strategies;
using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Query;

/// <summary>
/// Provides query mediation capabilities by coordinating the execution of queries
/// through the underlying message mediator framework.
/// </summary>
public class QueryMediator : IQueryMediator
{
    private readonly IMessageMediator _messageMediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryMediator"/> class with the specified message mediator.
    /// </summary>
    /// <param name="messageMediator">The message mediator instance used for coordinating query processing.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageMediator"/> is null.</exception>
    public QueryMediator(IMessageMediator messageMediator)
    {
        _messageMediator = messageMediator ?? throw new ArgumentNullException(nameof(messageMediator));
    }

    /// <summary>
    /// Asynchronously executes a query and returns the result using a single handler strategy.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of result returned by the query.</typeparam>
    /// <param name="query">The query to execute.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous query execution with the result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is null.</exception>
    public async Task<TQueryResult> SendAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var strategy = new MessageMediatorPipelineRequestHandlerStrategy<IQuery<TQueryResult>, TQueryResult>();
        var options = new MessageMediatorOptions<IQuery<TQueryResult>, Task<TQueryResult>>()
        {
            Strategy = strategy,
            CancellationToken = cancellationToken,
        };

        return await _messageMediator.Mediate(query, options);
    }
}
